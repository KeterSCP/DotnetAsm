<template>
    <q-dialog v-model="showAsmSummary" position="right" seamless full-height>
        <q-card>
            <q-card-section class="row items-center q-pb-none">
                <div class="text-h6">ASM Summary</div>
                <q-space />
                <q-btn icon="keyboard_arrow_right" flat round dense v-close-popup />
            </q-card-section>

            <q-card-section class="row items-center no-wrap">
                <q-scroll-area style="height: 790px; width: 700px">
                    <q-list bordered separator>
                        <q-item v-for="item in asmSummary" :key="item" clickable v-ripple @click="selectJitCompiledMethod(item)">
                            <q-item-section :set="(itemSplit = item.replace(']', '').split('['))">
                                <q-item-label>{{ itemSplit[0] }}</q-item-label>
                                <q-item-label caption>{{ itemSplit[1] }}</q-item-label>
                            </q-item-section>
                        </q-item>
                    </q-list>
                </q-scroll-area>
            </q-card-section>

            <q-card-section>
                <small>You can click on item to paste a method to "Method to compile" input.</small>
            </q-card-section>
        </q-card>
    </q-dialog>
    <div class="row q-px-md">
        <div class="col q-py-sm">
            <div class="row justify-between items-center q-pr-sm">
                <q-input outlined dense v-model="methodToCompile" label="Method to compile" style="max-width: 400px">
                    <template v-slot:append>
                        <q-icon name="help">
                            <q-tooltip class="text-body2">
                                You can use <strong>*</strong> as a wildcard character.
                                <br />
                                <br />
                                Examples:
                                <ul>
                                    <li>
                                        TestMethod* will search for any method name that <strong>starts with</strong> TestMethod
                                    </li>
                                    <li>
                                        *TestMethod will search for any method name that <strong>ends with</strong> TestMethod
                                    </li>
                                    <li>*TestMethod* will search for every method name <strong>containing</strong> TestMethod</li>
                                </ul>
                            </q-tooltip>
                        </q-icon>
                    </template>
                </q-input>
                <q-btn outline color="primary" style="height: 28px" @click="resetEditorContent"> Reset content </q-btn>
            </div>
        </div>
        <q-separator vertical />
        <div class="col q-py-sm">
            <q-checkbox v-model="useTieredCompilation" :disable="usePgo" label="Use tiered JIT">
                <q-tooltip class="text-body2">
                    Make sure to apply <strong>[MethodImpl(MethodImplOptions.NoInlining)]</strong> attribute on your method if
                    tiered JIT is not enabled
                    <div v-if="usePgo">
                        <br />
                        <span>* PGO mode makes sense only when running with tiered compilation</span>
                    </div>
                </q-tooltip>
            </q-checkbox>
            <q-checkbox v-model="usePgo" label="Use PGO" @update:model-value="onPgoChecked" />
            <q-checkbox v-model="useReadyToRun" label="Use ReadyToRun">
                <q-tooltip class="text-body2">
                    If <strong>true</strong>, then compilation will use precompiled .NET libraries code.
                    <br />
                    Setting it to <strong>false</strong> will slow down compilation time, but will allow more optimizations when
                    running in PGO mode (libraries code will be recompiled from Tier-0 with instrumentation)
                </q-tooltip>
            </q-checkbox>
            <span>
                <q-tooltip v-if="!methodToCompile" class="text-body2"> Fill the "Method to compile" field first </q-tooltip>
                <q-btn
                    :loading="loading"
                    :disable="!methodToCompile"
                    outline
                    color="primary"
                    class="q-ml-md"
                    @click="generateAsmCode"
                >
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
        </div>
    </div>
    <div class="row justify-between">
        <div class="col">
            <v-ace-editor
                v-model:value="csharpCode"
                lang="csharp"
                :theme="darkTheme ? 'vibrant_ink' : 'chrome'"
                style="height: 88vh; resize: both"
                :printMargin="false"
                :options="{
                    enableBasicAutocompletion: true,
                    enableSnippets: true,
                    enableLiveAutocompletion: true,
                }"
            />
        </div>
        <q-separator vertical />

        <div class="col">
            <v-ace-editor
                v-model:value="asmCode"
                readonly
                lang="assembly_x86"
                :theme="darkTheme ? 'vibrant_ink' : 'chrome'"
                style="height: 88vh; resize: both"
                :printMargin="false"
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
    <q-separator />
    <div class="q-pa-xs float-right">
        <q-checkbox v-model="darkTheme" label="Dark mode" />
    </div>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import { VAceEditor } from "src/components/VAceEditor";
import AsmGenerationRequest from "src/models/AsmGenerationRequest";
import AsmGenerationResponse from "src/models/AsmGenerationResponse";
import { Dark } from "quasar";

import "ace-builds/src-noconflict/mode-csharp";
import "ace-builds/src-noconflict/mode-assembly_x86";
import "ace-builds/src-noconflict/theme-vibrant_ink";
import "ace-builds/src-noconflict/theme-chrome";

const defaultEditorContent = `using System;
using System.Runtime.CompilerServices;

// [MethodImpl(MethodImplOptions.NoInlining)]


for (int i = 0; i < 100; i++)
{
    // Insert method call here (do not remove Thread.Sleep)

    Thread.Sleep(10);
}`;

const csharpCode = ref(localStorage.getItem("cached-code") ?? defaultEditorContent);
const loading = ref(false);
const asmCode = ref("");
const asmSummary = ref([] as string[]);
const errorsDialog = ref(false);
const errors = ref("");

const usePgo = ref(false);
const useTieredCompilation = ref(true);
const useReadyToRun = ref(true);
const methodToCompile = ref("");
const showAsmSummary = ref(false);

const themeDarkStorageName = "theme-dark";
const darkTheme = ref(Boolean(JSON.parse(localStorage.getItem(themeDarkStorageName) ?? "true")));
updateTheme(darkTheme.value);

watch(darkTheme, (currentValue: boolean) => {
    updateTheme(currentValue);
    localStorage.setItem(themeDarkStorageName, JSON.stringify(currentValue));
});

function updateTheme(darkTheme: boolean) {
    Dark.set(darkTheme);
}

async function generateAsmCode() {
    const request = new AsmGenerationRequest(
        csharpCode.value,
        methodToCompile.value,
        false,
        usePgo.value,
        useTieredCompilation.value,
        useReadyToRun.value
    );

    loading.value = true;

    const response = await fetch("/api/generate-asm", {
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
