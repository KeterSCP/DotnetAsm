<template>
    <div id="editor" ref="editor" />
</template>

<script setup lang="ts">
import loader from "@monaco-editor/loader";
import { editor } from "monaco-editor";
import { onMounted, PropType, toRefs } from "vue";

import IStandaloneEditorConstructionOptions = editor.IStandaloneEditorConstructionOptions;

const props = defineProps({
    options: {
        type: Object as PropType<IStandaloneEditorConstructionOptions>,
        required: true,
    },
});

const { options } = toRefs(props);

onMounted(() => {
    loader.init().then((monaco) => {
        // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
        monaco.editor.create(document.getElementById("editor")!, options.value);
    });
});
</script>

<style scoped>
#editor {
    width: 100vw;
    height: 100vh;
}
</style>
