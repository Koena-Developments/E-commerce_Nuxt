<template>
  <div>
    <button v-if="loggedIn" @click="handleSignOut">Sign Out</button>
    <button v-else @click="handleSignIn">Sign In</button>

    <p>Status: {{ status }}</p>
  </div>
</template>

<script setup>
// import { computed } from 'vue'; // Make sure to import computed
// import { useAuth } from '#auth'; // Assuming this is correct for your Nuxt Auth setup

const { status, signIn, signOut, data } = useAuth();

const loggedIn = computed(() => status.value === 'authenticated');

async function handleSignIn() {
  try {
    await signIn('github');
    if (status.value === 'authenticated' && data.value?.user) {
      console.log('Successfully signed in!', data.value.user);
      await navigateTo('/');
    } else {
      console.log('Sign-in process completed, but not authenticated or user data missing.');
    }
  } catch (error) {
    console.error('Sign-in failed:', error);
  }
}

async function handleSignOut() {
  try {
    await signOut();
    console.log('Successfully signed out!');
    await navigateTo('/login'); 
  } catch (error) {
    console.error('Sign-out failed:', error);
  }
}


</script>

<style scoped>
div {
  margin: 20px;
  text-align: center;
}
button {
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 1em;
}
button:hover {
  background-color: #0056b3;
}
</style>