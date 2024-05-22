<template>
  <asm-summary-dialog v-model="showAsmSummary" :asm-summary="asmSummary" @selectedJitCompiledMethod="selectJitCompiledMethod" />
  <diff-tool-dialog v-model="showDiffTool" />
  <div class="row q-px-md">
    <div class="col q-py-sm">
      <div class="row justify-between items-center q-pr-sm">
        <q-input outlined dense v-model="methodToCompile" label="Method to compile" style="max-width: 400px" clearable />
        <q-btn outline color="primary" style="height: 28px" @click="resetEditorContent"> Reset content </q-btn>
      </div>
    </div>
    <div class="col q-py-sm">
      <q-checkbox v-model="useTieredCompilation" :disable="usePgo" label="Tiered JIT">
        <q-tooltip class="text-body2">
          Make sure to apply <strong>[MethodImpl(MethodImplOptions.NoInlining)]</strong> attribute on your method if tiered JIT is
          not enabled
          <div v-if="usePgo">
            <br />
            <span>* PGO mode makes sense only when running with tiered compilation</span>
          </div>
        </q-tooltip>
      </q-checkbox>
      <q-checkbox v-model="usePgo" label="PGO" @update:model-value="onPgoChecked" />
      <q-checkbox v-model="useReadyToRun" label="ReadyToRun">
        <q-tooltip class="text-body2">
          If <strong>true</strong>, then compilation will use precompiled .NET libraries code.
          <br />
          Setting it to <strong>false</strong> will slow down compilation time, but will allow more optimizations when running in
          PGO mode (libraries code will be recompiled from Tier-0 with instrumentation)
        </q-tooltip>
      </q-checkbox>

      <q-select
        v-model="selectedTargetFramework"
        :options="targetFrameworks"
        label="Target framework"
        outlined
        dense
        style="width: 200px; display: inline-flex; padding-left: 10px"
      />

      <span>
        <q-tooltip v-if="!methodToCompile" class="text-body2"> Fill the "Method to compile" field first </q-tooltip>
        <q-btn :loading="loading" :disable="!methodToCompile" outline color="primary" class="q-ml-md" @click="generateAsmCode">
          Generate ASM
          <template #loading>
            <q-spinner-gears color="primary" />
          </template>
        </q-btn>
      </span>
      <q-btn
        :disable="!asmCode"
        outline
        label="Show ASM Summary"
        color="primary"
        class="q-ml-md"
        @click="showAsmSummary = !showAsmSummary"
      />
      <q-btn outline label="Open diff-tool" color="primary" class="q-ml-md" @click="showDiffTool = !showDiffTool" />
    </div>
  </div>
  <div class="row justify-between q-px-sm">
    <div class="col">
      <VMonacoEditor
        v-model="csharpCode"
        id="csharp-editor"
        :options="{
          language: csharpLanguageName,
          automaticLayout: true,
          theme: 'csharp-semantic-theme',
          'semanticHighlighting.enabled': true,
        }"
        style="height: 88vh; resize: both"
      />
    </div>
    <div class="col">
      <VMonacoEditor
        v-model="asmCode"
        id="asm-editor"
        :options="{
          language: ryujitXarchAsm,
          readOnly: true,
          automaticLayout: true,
        }"
        style="height: 88vh; resize: both"
      />
    </div>
  </div>
  <q-dialog v-model="errorsDialog" fullWidth>
    <q-card>
      <q-card-section>
        <span class="text-h6">Something went wrong</span>
      </q-card-section>

      <q-card-section class="q-pt-none" style="white-space: break-spaces">
        {{ errors }}
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat label="OK" color="primary" v-close-popup />
      </q-card-actions>
    </q-card>
  </q-dialog>
  <div class="q-pa-xs float-right">
    <q-checkbox v-model="darkTheme" label="Dark mode" />
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import AsmSummaryDialog from "src/components/AsmSummaryDialog.vue";
import DiffToolDialog from "src/components/DiffToolDialog.vue";
import TargetFramework from "src/models/TargetFramework";
import VMonacoEditor from "src/components/VMonacoEditor.vue";
import AsmGenerationRequest from "src/models/AsmGenerationRequest";
import AsmGenerationResponse from "src/models/AsmGenerationResponse";
import darkTheme from "src/theme/dark-theme";
import { languageName as ryujitXarchAsm } from "../monaco/ryujit-xarch-asm-lang";
import { languageName as csharpLanguageName } from "src/monaco/csharp-semantic-lang";

const defaultEditorContent = `using System;
using System.Threading;
using System.Runtime.CompilerServices;

// [MethodImpl(MethodImplOptions.NoInlining)]

for (int i = 0; i < 100; i++)
{
    // Insert method call here (do not remove Thread.Sleep)

    Thread.Sleep(10);
}
`;

const csharpCode = ref(localStorage.getItem("cached-code") ?? defaultEditorContent);
const loading = ref(false);
const asmCode = ref("");
const asmSummary = ref([] as string[]);
const errorsDialog = ref(false);
const errors = ref("");

const usePgo = ref(true);
const useTieredCompilation = ref(true);
const useReadyToRun = ref(true);
const methodToCompile = ref("");
const showAsmSummary = ref(false);
const showDiffTool = ref(false);
const targetFrameworks = [{ label: ".NET 8", value: "net80" }];
const selectedTargetFramework = ref({ label: ".NET 8", value: "net80" });

async function generateAsmCode() {
  const request = new AsmGenerationRequest(
    csharpCode.value,
    methodToCompile.value,
    false,
    usePgo.value,
    useTieredCompilation.value,
    useReadyToRun.value,
    selectedTargetFramework.value.value as TargetFramework
  );

  loading.value = true;

  const response = await fetch(`${process.env.BACKEND_URL}/api/generate-asm`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  const responseText = await response.text();
  const generationResponse = JSON.parse(responseText) as AsmGenerationResponse;

  if (generationResponse.errors) {
    errors.value = generationResponse.errors;
    errorsDialog.value = true;
  }

  asmCode.value = generationResponse.asm;
  asmSummary.value = generationResponse.asmSummary;
  loading.value = false;

  localStorage.setItem("cached-code", csharpCode.value);
}

function selectJitCompiledMethod(jittedMethodInfo: string) {
  const methodName = jittedMethodInfo.substring(jittedMethodInfo.indexOf("d ") + 2, jittedMethodInfo.indexOf("("));
  methodToCompile.value = methodName;
  showAsmSummary.value = false;
}

function onPgoChecked(pgoChecked: boolean) {
  if (pgoChecked) {
    useTieredCompilation.value = true;
  }
}

function resetEditorContent() {
  csharpCode.value = defaultEditorContent;
}
</script>
