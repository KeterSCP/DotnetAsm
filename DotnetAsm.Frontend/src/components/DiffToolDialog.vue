<template>
    <q-dialog
        :model-value="modelValue"
        @show="onShow"
        @before-hide="onBeforeHide"
        @update:model-value="(value) => emits('update:modelValue', value)"
    >
        <div id="diff-editor" />
    </q-dialog>
</template>

<script setup lang="ts">
import { toRefs, onMounted } from "vue";
import { editor } from "monaco-editor";
import {
    languageName as ryujitXarchAsm,
    themeName as ryujitXarchTheme,
    themeNameDark as ryujitXarchThemeDark,
} from "../monaco/ryujit-xarch-asm-lang";
import darkTheme from "src/theme/dark-theme";

const props = defineProps({
    modelValue: {
        type: Boolean,
        required: true,
    },
});

const emits = defineEmits(["update:modelValue"]);

const { modelValue } = toRefs(props);
let internalEditor: editor.IStandaloneDiffEditor | null = null;

let originalModel: editor.ITextModel;
let changedModel: editor.ITextModel;

onMounted(() => {
    originalModel = editor.createModel("", ryujitXarchAsm);
    changedModel = editor.createModel("", ryujitXarchAsm);
});

function onShow() {
    // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
    const editorContainer = document.getElementById("diff-editor")!;

    if (!internalEditor) {
        internalEditor = editor.createDiffEditor(editorContainer, {
            theme: darkTheme.value ? ryujitXarchThemeDark : ryujitXarchTheme,
            renderOverviewRuler: false,
            originalEditable: true,
            minimap: { enabled: false },
        });
    }

    internalEditor.setModel({
        // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
        original: originalModel!,
        // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
        modified: changedModel!,
    });
}

function onBeforeHide() {
    internalEditor?.dispose();
    internalEditor = null;
}
</script>

<style scoped>
#diff-editor {
    max-width: none;
    height: 100vh;
    width: 80vw;
    margin: 10px 10px 10px 10px;
}
</style>
