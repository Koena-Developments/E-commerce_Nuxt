<template>
  <div>
    <button v-if="loggedIn" @click="handleSignOut">Sign Out</button>
    <button v-else @click="handleSignIn">Sign In</button>

    <p>Status: {{ status }}</p>
  </div>
</template>

<script setup>
import { computed } from 'vue'; // Make sure to import computed
import { useAuth } from '#auth'; // Assuming this is correct for your Nuxt Auth setup

const { status, signIn, signOut, data } = useAuth(); // Also get 'data' to check for user

const loggedIn = computed(() => status.value === 'authenticated');

async function handleSignIn() {
  try {
    // Attempt to sign in. You can specify a provider here, e.g., 'github'
    // This will typically redirect the user to the provider's login page
    // and then back to your app.
    await signIn('github'); // Example: sign in with GitHub

    // After signIn completes (either successful or failed redirection/callback)
    // check the status or data to determine navigation.
    if (status.value === 'authenticated' && data.value?.user) {
      console.log('Successfully signed in!', data.value.user);
      // If signed in, navigate to the home page or a dashboard
      // Use Nuxt's navigateTo for client-side navigation
      await navigateTo('/');
    } else {
      console.log('Sign-in process completed, but not authenticated or user data missing.');
      // If not signed in after the attempt (e.g., user closed popup, or cancelled)
      // You might navigate to a registration page or show an error message
      // For now, let's log, but you could navigate if needed
      // await navigateTo('/register'); // Example: navigate to registration
    }
  } catch (error) {
    console.error('Sign-in failed:', error);
    // Handle errors during the sign-in process
    // Maybe navigate to an error page or show a toast notification
    // await navigateTo('/login-error');
  }
}

async function handleSignOut() {
  try {
    // Sign out the user. This often clears the session and redirects.
    await signOut();
    console.log('Successfully signed out!');
    // After signing out, you might want to redirect to the login page or home page
    await navigateTo('/login'); // Example: redirect to a login page
  } catch (error) {
    console.error('Sign-out failed:', error);
    // Handle sign-out errors
  }
}

// Optional: Add a watch to react to authentication status changes
// watch(status, (newStatus) => {
//   console.log('Authentication status changed to:', newStatus);
//   if (newStatus === 'authenticated') {
//     // Perform actions when user authenticates
//   } else if (newStatus === 'unauthenticated') {
//     // Perform actions when user becomes unauthenticated
//   }
// });
</script>

<style scoped>
/* Basic styling for the button */
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