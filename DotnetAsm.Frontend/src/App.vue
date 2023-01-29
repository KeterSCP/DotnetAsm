<template>
    <router-view />
</template>

<script setup lang="ts">
import editorWorker from "monaco-editor/esm/vs/editor/editor.worker?worker";
import "./monaco/ryujit-xarch-asm-lang";

import { onMounted, watch } from "vue";
import { Dark } from "quasar";
import darkTheme, { saveTheme } from "src/theme/dark-theme";

self.MonacoEnvironment = {
    getWorker() {
        return new editorWorker();
    },
};

onMounted(() => {
    updateTheme(darkTheme.value);
});

watch(darkTheme, (currentValue: boolean) => {
    updateTheme(currentValue);
});

function updateTheme(darkTheme: boolean) {
    Dark.set(darkTheme);
    saveTheme(darkTheme);
}
</script>
