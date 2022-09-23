<template>
    <div class="row q-px-md">
        <div class="col q-py-sm">
            <q-input outlined dense v-model="methodToCompile" label="Method to compile" style="max-width: 400px">
                <template v-slot:append>
                    <q-icon name="help">
                        <q-tooltip class="text-body2">
                            You can use <strong>*</strong> as a wildcard character.
                            <br />
                            <br />
                            Examples:
                            <ul>
                                <li>TestMethod* will search for any method name that <strong>starts with</strong> TestMethod</li>
                                <li>*TestMethod will search for any method name that <strong>ends with</strong> TestMethod</li>
                                <li>*TestMethod* will search for every method name <strong>containing</strong> TestMethod</li>
                            </ul>
                        </q-tooltip>
                    </q-icon>
                </template>
            </q-input>
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
        </div>
    </div>
    <div class="row justify-between">
        <div class="col">
            <v-ace-editor
                v-model:value="csharpCode"
                lang="csharp"
                theme="vibrant_ink"
                style="height: 90vh; resize: both"
                :printMargin="false"
            />
        </div>
        <q-separator vertical />

        <div class="col">
            <v-ace-editor
                v-model:value="asmCode"
                readonly
                lang="assembly_x86"
                theme="vibrant_ink"
                style="height: 90vh; resize: both"
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
</template>

<script setup lang="ts">
import { ref } from "vue";
import { VAceEditor } from "vue3-ace-editor";
import "ace-builds/src-noconflict/mode-csharp";
import "ace-builds/src-noconflict/mode-assembly_x86";
import "ace-builds/src-noconflict/theme-vibrant_ink";
import AsmGenerationRequest from "src/models/AsmGenerationRequest";
import AsmGenerationResponse from "src/models/AsmGenerationResponse";

const csharpCode = ref(
    localStorage.getItem("cached-code") ??
        `using System;
using System.Runtime.CompilerServices;

// [MethodImpl(MethodImplOptions.NoInlining)]


for (int i = 0; i < 100; i++)
{
    // Insert method call here (do not remove Thread.Sleep)

    Thread.Sleep(10);
}`
);

const loading = ref(false);
const asmCode = ref("");
const errorsDialog = ref(false);
const errors = ref("");

const usePgo = ref(false);
const useTieredCompilation = ref(true);
const useReadyToRun = ref(true);
const methodToCompile = ref("");

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
    loading.value = false;

    localStorage.setItem("cached-code", csharpCode.value);
}

function onPgoChecked(pgoChecked: boolean) {
    if (pgoChecked) {
        useTieredCompilation.value = true;
    }
}
</script>
