<template>
  <button @click="handleLogout">Logout</button>
</template>

<script setup>
import { useAuth } from '#app';
const { signOut } = useAuth();

const handleLogout = async () => {
  await signOut();
  navigateTo('/Auth/login');
};
</script>