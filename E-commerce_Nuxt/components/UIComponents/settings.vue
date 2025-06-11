<template>
  <button class="settings-btn" @click="goToSettings">
    Settings
  </button>
</template>

<script setup lang="ts">
import { navigateTo } from '#app'; 

const goToSettings = () => {
  navigateTo('/settings');
};
</script>

<style scoped>
.settings-btn {
  width: 100%; 
  padding: 10px 15px;
  background-color: #007bff; 
  color: white;
  border: none;
  border-radius: 6px; 
  font-size: 16px;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.2s ease, transform 0.1s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); 
  margin-bottom: 40px;
}

.settings-btn:hover {
  background-color: #0056b3;
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

.settings-btn:active {
  transform: translateY(0); 
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}
</style>
