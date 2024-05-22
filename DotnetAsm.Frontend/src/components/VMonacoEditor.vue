<template>
  <div :id="id" />
</template>

<script setup lang="ts">
import { editor } from "monaco-editor";
import { onMounted, PropType, toRefs, onBeforeUnmount, watch, ref } from "vue";
import darkTheme from "src/theme/dark-theme";
import { themeName, themeNameDark } from "src/monaco/DotnetAsmTheme";

import IStandaloneEditorConstructionOptions = editor.IStandaloneEditorConstructionOptions;
import registerCSharpLanguageProvider from "src/monaco/c-sharp-language.provider";

const props = defineProps({
  modelValue: {
    type: String,
    required: true,
  },
  id: {
    type: String,
    required: true,
  },
  options: {
    type: Object as PropType<IStandaloneEditorConstructionOptions>,
    required: true,
  },
});

const emits = defineEmits(["update:modelValue"]);
const { options, modelValue, id } = toRefs(props);

let internalEditor: editor.IStandaloneCodeEditor | null = null;

const contentBackup = ref("");
const isSettingContent = ref(false);

onMounted(() => {
  if (options.value.language === "csharp-semantic") {
    registerCSharpLanguageProvider();
  }

  contentBackup.value = internalEditor?.getValue() ?? "";

  options.value.theme = darkTheme.value ? themeNameDark : themeName;
  options.value.value = modelValue.value;

  // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
  internalEditor = editor.create(document.getElementById(id.value)!, options.value);
  internalEditor.onDidChangeModelContent(() => {
    const val = internalEditor?.getValue();
    contentBackup.value = val ?? "";
    emits("update:modelValue", val);
  });
});

onBeforeUnmount(() => {
  internalEditor?.dispose();
  internalEditor = null;
});

watch(modelValue, (newValue) => {
  if (contentBackup.value !== newValue) {
    try {
      isSettingContent.value = true;
      internalEditor?.setValue(newValue);
    } finally {
      isSettingContent.value = false;
    }
    contentBackup.value = newValue;
  }
});

watch(darkTheme, (dark) => {
  internalEditor?.updateOptions({
    theme: dark ? themeNameDark : themeName,
  });
});
</script>
