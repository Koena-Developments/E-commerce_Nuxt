<template>
  <aside class="filter-sidebar" :class="{ visible: visible }">
    <div class="filter-header">
      <h3>Categories</h3>
      <button class="close-btn" @click="$emit('toggle')">×</button>
    </div>
    <ul class="category-list">
      <li
        v-for="category in categories"
        :key="category"
        :class="{ active: category === selected }"
        @click="$emit('select', category)"
      >
        {{ category }}
      </li>
    </ul>

    <div class="settings-button-wrapper">
      <SettingsButton />
    </div>
  </aside>
</template>
 
<script setup>
import SettingsButton from '~/components/UIComponents/Settingsbutton';

defineProps({
  categories: Array,
  selected: String,
  visible: Boolean
});

defineEmits(['select', 'toggle']);
</script>

<style scoped>
.filter-sidebar {
  position: fixed;
  top: 0;
  left: 0;
  width: 200px;
  height: 100vh;
  background: #f8f9fa;
  border-right: 1px solid #ccc;
  padding: 20px;
  box-shadow: 2px 0 6px rgba(0, 0, 0, 0.1);
  transform: translateX(-100%);
  transition: transform 0.3s ease;
  z-index: 999;
  display: flex;
  flex-direction: column;
}

.filter-sidebar.visible {
  transform: translateX(0);
}

.filter-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.filter-header h3 {
  margin: 0;
  font-size: 18px;
}

.close-btn {
  background: transparent;
  border: none;
  font-size: 20px;
  cursor: pointer;
}

.category-list {
  list-style: none;
  padding: 0;
  margin: 0;
  flex-grow: 1;
  overflow-y: auto;
  margin-bottom: 20px;
}

.category-list li {
  padding: 10px;
  cursor: pointer;
  border-radius: 4px;
  transition: background 0.2s;
}

.category-list li:hover {
  background: #e2e6ea;
}

.category-list li.active {
  background: #007bff;
  color: white;
  font-weight: bold;
}

.settings-button-wrapper {
  margin-top: auto;
  padding-top: 10px;
  border-top: 1px solid #eee;
}
</style>