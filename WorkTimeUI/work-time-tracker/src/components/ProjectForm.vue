<script setup>
import { defineProps, defineEmits, ref, watch } from 'vue';
import { updateProject, createProject } from '@/services/projects';
import { NInput, NCheckbox, NSpace, NButton, NForm, NFormItem, useMessage } from 'naive-ui';

const props = defineProps({ project: Object });
const emit = defineEmits(['update', 'close']);
const message = useMessage();

const editedProject = ref({ ...props.project });

watch(() => props.project, (newProject) => {
    editedProject.value = { ...newProject };
});

const saveProject = async () => {
    try {
        if (editedProject.value.id) {
            await updateProject(editedProject.value.id, editedProject.value);
        } else {
            await createProject(editedProject.value);
        }
        emit('update');
        emit('close');
    } catch (error) {
        message.error('Ошибка сохранения проекта');
    }
};

</script>

<template>
    <n-form label-placement="left">
        <n-space horizontal justify="center">
            <n-form-item label="Название проекта">
                <n-input v-model="editedProject.name" :default-value="editedProject.name" type="text"
                    placeholder="Название проекта" @update:value="(newValue) => editedProject.name = newValue" />
            </n-form-item>
            <n-form-item label="Активность">
                <n-checkbox v-model="editedProject.isActive" :checked="editedProject.isActive"
                    @update:checked="(newValue) => editedProject.isActive = newValue">
                </n-checkbox>
            </n-form-item>

            <n-form-item label="Код проекта">
                <n-input v-model="editedProject.code" :default-value="editedProject.code" type="text" placeholder="Код проекта"
            @update:value="(newValue) => editedProject.code = newValue" />
            </n-form-item>
            <n-form-item>
                <n-button size="small" type="success" @click="saveProject">
                    {{ editedProject.id ? 'Сохранить' : 'Создать' }}
                </n-button>
            </n-form-item>
            <n-form-item>
                <n-button size="small" type="error" @click="$emit('close')">Закрыть</n-button>
            </n-form-item>
        </n-space>
    </n-form>
</template>
