<template>
  <button @click="handleLogout">
    Logout </button>
</template>

<script setup>
import { navigateTo } from '#app';
const { signOut } = useAuth();

const handleLogout = async () => {
  await signOut();
  navigateTo('/Auth/login');
};
</script>

<style scoped>
button {

  position: fixed; 
  top: 20px; 
  right: 20px; 
  z-index: 1000; 

  height: 50px;
  min-width: 100px; 
  padding: 0 20px; 
  border-radius: 25px; 
  border: none; 

  background-color: #e74c3c; 
  color: white; 

  font-size: 16px;
  font-weight: bold;
  text-transform: uppercase; 
  letter-spacing: 0.5px; 

  cursor: pointer;
  transition: background-color 0.3s ease, transform 0.2s ease, box-shadow 0.3s ease; /* Smooth transitions */
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2); 
  display: flex;
  justify-content: center;
  align-items: center;
}

button:hover {
  background-color: #c0392b;
  transform: translateY(-2px); 
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.3);
}

button:active {
  transform: translateY(0); 
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2); 
}
</style>