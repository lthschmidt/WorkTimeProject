<script setup>
import { defineProps, defineEmits, ref, watch } from 'vue';
import { updateTask, createTask } from '@/services/tasks';
import { NForm, NFormItem, NInput, NCheckbox, NSpace, NButton, NInputNumber, useMessage } from 'naive-ui';
import { useRoute } from 'vue-router';

const props = defineProps({ task: Object });
const emit = defineEmits(['update', 'close']);
const route = useRoute();
const message = useMessage();

const editedTask = ref(
    route.params.projectID ? { projectId: Number(route.params.projectID) } : { ...props.task }
);

// Обновляем editedTask при изменении props.task
watch(() => props.task, (newTask) => {
    editedTask.value = { ...newTask };
});

const saveTask = async () => {
    try {
        if (editedTask.value.id) {
            await updateTask(editedTask.value.id, editedTask.value);
        }
        else {
            await createTask(editedTask.value);
        }
        emit('update');
        emit('close');
    } catch (error) {
        message.error('Ошибка обновления проекта');
    }
};

</script>

<template>
    <n-form label-placement="left" >
        <n-space horizontal justify="center">
            <n-form-item label="Название задачи">
                <n-input v-model="editedTask.name" :default-value="editedTask.name" type="text"
                    placeholder="Название задачи" @update:value="(newValue) => editedTask.name = newValue" />
            </n-form-item>

            <n-form-item label="Активность">
                <n-checkbox v-model="editedTask.isActive" :checked="editedTask.isActive"
                    @update:checked="(newValue) => editedTask.isActive = newValue" />
            </n-form-item>

            <n-form-item label="ID проекта">
                <n-input-number v-model:value="editedTask.projectId" :default-value="editedTask.projectId"
                    placeholder="ID проекта" @update:value="(newValue) => editedTask.projectId = newValue"
                    :show-button="false" clearable />
            </n-form-item>
            <n-form-item>
                <n-button size="small" type="success" @click="saveTask">
                    {{ editedTask.id ? 'Сохранить' : 'Создать' }}
                </n-button>
            </n-form-item>
            <n-form-item>
                <n-button size="small" type="error" @click="$emit('close')">Закрыть</n-button>
            </n-form-item>
        </n-space>
    </n-form>
</template>
