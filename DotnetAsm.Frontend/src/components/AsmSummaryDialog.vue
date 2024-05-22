<template>
  <q-dialog v-model="modelValue" position="right" seamless full-height>
    <q-card>
      <q-card-section class="row items-center q-pb-none">
        <div class="text-h6">ASM Summary</div>
        <q-space />
        <q-btn icon="keyboard_arrow_right" flat round dense @click="emits('update:modelValue', false)" />
      </q-card-section>

      <q-card-section class="row items-center no-wrap">
        <q-scroll-area style="height: 790px; width: 700px">
          <q-list bordered separator>
            <q-item v-for="item in asmSummary" :key="item" clickable v-ripple @click="emits('selectedJitCompiledMethod', item)">
              <q-item-section>
                <q-item-label>{{ getAsmSummaryLabel(item) }}</q-item-label>
                <q-item-label caption>{{ getAsmSummaryCaption(item) }}</q-item-label>
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
</template>

<script setup lang="ts">
import { toRefs } from "vue";

const props = defineProps({
  modelValue: {
    type: Boolean,
    required: true,
  },
  asmSummary: {
    type: Array<string>,
    required: true,
  },
});

const emits = defineEmits(["selectedJitCompiledMethod", "update:modelValue"]);

const { modelValue, asmSummary } = toRefs(props);

function getAsmSummaryLabel(methodCompilationSummary: string): string {
  const firstBracketIndex = methodCompilationSummary.indexOf("[");
  return methodCompilationSummary.slice(0, firstBracketIndex);
}

function getAsmSummaryCaption(methodCompilationSummary: string): string {
  const firstBracketIndex = methodCompilationSummary.indexOf("[");

  return methodCompilationSummary.slice(firstBracketIndex); //.replaceAll(/[\[\]]/g, "");
}
</script>
