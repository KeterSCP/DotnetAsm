<template>
    <div :id="id" />
</template>

<script setup lang="ts">
import { editor } from "monaco-editor";
import { onMounted, PropType, toRefs, onBeforeUnmount, watch, ref } from "vue";
import darkTheme from "src/theme/dark-theme";

import IStandaloneEditorConstructionOptions = editor.IStandaloneEditorConstructionOptions;

const props = defineProps({
    modelValue: {
        type: String,
        required: true,
    },
    id: {
        type: String,
        required: true,
    },
    darkThemeName: {
        type: String,
        required: true,
    },
    lightThemeName: {
        type: String,
        required: true,
    },
    options: {
        type: Object as PropType<IStandaloneEditorConstructionOptions>,
        required: true,
    },
});

const emits = defineEmits(["update:modelValue"]);
const { options, modelValue, id, darkThemeName, lightThemeName } = toRefs(props);

let internalEditor: editor.IStandaloneCodeEditor | null = null;
let textModel: editor.ITextModel | null = null;

const contentBackup = ref("");
const isSettingContent = ref(false);

onMounted(() => {
    textModel = editor.createModel(modelValue.value, options.value.language);
    textModel.onDidChangeContent(() => {
        const val = textModel?.getValue();
        contentBackup.value = val ?? "";
        emits("update:modelValue", val);
    });

    contentBackup.value = textModel.getValue();

    options.value.theme = darkTheme.value ? darkThemeName.value : lightThemeName.value;

    // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
    internalEditor = editor.create(document.getElementById(id.value)!, options.value);

    internalEditor.setModel(textModel);
});

onBeforeUnmount(() => {
    internalEditor?.dispose();
    internalEditor = null;
});

watch(modelValue, (newValue) => {
    if (contentBackup.value !== newValue) {
        try {
            isSettingContent.value = true;
            textModel?.setValue(newValue);
        } finally {
            isSettingContent.value = false;
        }
        contentBackup.value = newValue;
    }
});

watch(darkTheme, (dark) => {
    internalEditor?.updateOptions({
        theme: dark ? darkThemeName.value : lightThemeName.value,
    });
});
</script>
