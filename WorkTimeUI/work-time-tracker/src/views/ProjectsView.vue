<script setup>
import { ref, onMounted, h, inject } from 'vue';
import { useRouter } from 'vue-router';
import { getProjects, deleteProject } from '@/services/projects';
import ProjectForm from '@/components/ProjectForm.vue';
import { NDataTable, NButton, NTag, useMessage, NModal, NLayout, NH2, NFlex } from 'naive-ui';

const projects = ref([]);
// Редактируемый проект
const selectedProject = ref(null);
const router = useRouter();
const message = useMessage();
// Состояние модального окна
const showModal = inject('showModal');

// Функция загрузки проектов
const fetchProjects = async () => {
    try {
        const response = await getProjects();
        projects.value = response.data.map((p, index) => ({
            index: index + 1,
            ...p
        }));
    } catch (error) {
        message.error('Ошибка загрузки проектов');
    }
};
onMounted(fetchProjects);

// Удаление проекта
const removeProject = async (id) => {
    if (!confirm('Вы уверены, что хотите удалить проект?')) return;
    try {
        await deleteProject(id);
        projects.value = projects.value.filter(p => p.id !== id);
        message.success('Проект удалён');
    } catch (error) {
        message.error('Ошибка удаления проекта');
    }
};

// Выбор проекта для редактирования
const selectProject = (project) => {
    if (selectedProject.value === null) {
        selectedProject.value = { ...project };
    } else {
        // При нажатии второй раз форма закрывается
        selectedProject.value = null;
    }
};

// Колонки таблицы
const columns = [
    {
        type: 'expand',
        expandable: (row) => {
            return selectedProject.value && row.id === selectedProject.value.id;
        },
        renderExpand: (row) => {
            return h(ProjectForm, {
                project: row,
                onUpdate: fetchProjects,
                onClose: () => (selectedProject.value = null) 
            });
        }
    },
    { title: '#', key: 'index', width: 'auto' },
    { title: 'Название', key: 'name', width: 'auto' },
    {
        title: 'Активен',
        key: 'isActive',
        align: 'center',
        width: 'auto',
        render: (row) =>
            row.isActive
                ? h(NTag, { type: 'success' }, { default: () => '✔' })
                : h(NTag, { type: 'error' }, { default: () => '✘' })
    },
    { title: 'Код', key: 'code', width: 'auto', },
    {
        title: 'Действия',
        key: 'actions',
        width: 'auto',
        render: (row) =>
            h('div', { style: `display: flex; gap: 20px; justify-content: flex-start;` }, [
                h(NButton, {
                    secondary: true,
                    type: 'primary',
                    size: 'small',
                    onClick: () => router.push(`/tasks/${row.id}`)
                }, () => 'К задачам'),
                h(NButton, {
                    secondary: true,
                    type: 'warning',
                    size: 'small',
                    onClick: () => selectProject(row)
                }, () => 'Редактировать'),
                h(NButton, {
                    secondary: true,
                    type: 'error',
                    size: 'small',
                    onClick: () => removeProject(row.id)
                }, () => 'Удалить')
            ])
    }
];
</script>

<template>
    <n-layout style="padding-right: 10px; padding-left: 10px;">
            <n-flex justify="space-between" align="center">
                <n-h2>Список проектов</n-h2>
                <n-button type="primary" size="medium" @click="() => showModal = !showModal">Добавить проект</n-button>
            </n-flex>

            <n-data-table :columns="columns" :data="projects" :bordered="false" :striped="true" default-expand-all style="margin-top: 10px;">
            </n-data-table>

            <!-- Модальное окно для добавления проекта -->
            <n-modal v-model:show="showModal" style="width: 1000px" preset="card" title="Добавление проекта"
                size="medium" >
                <ProjectForm :project="selectedProject" @update="fetchProjects" @close="() => showModal = !showModal" />
            </n-modal>
    </n-layout>
</template>
