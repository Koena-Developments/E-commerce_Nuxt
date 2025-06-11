<template>
  <nav class="navbar">
    <div class="navbar-container">
      <div class="user-info">
        <img
          :src="userImage"
          alt="User Avatar"
          class="user-avatar"
        />
        <span class="username">
          {{ displayUsername }} 
        </span>
      </div>

      <div class="nav-links">
        <NuxtLink to="/" class="nav-link">Home</NuxtLink>
        <NuxtLink to="/settings" class="nav-link">Settings</NuxtLink>
        <NuxtLink to="/track-order" class="nav-link">Track Order</NuxtLink>
      </div>

      <div class="logout-wrapper">
        <button
          @click="handleLogout"
          class="logout-button"
        >
          Logout
        </button>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, watch } from 'vue';
import { navigateTo } from '#app';

const { data: authData, signOut, status } = useAuth();

const displayUsername = ref("");
const userImage = ref("https://placehold.co/40x40/555555/FFFFFF?text=Gu"); 

watch(authData, (newData) => {
  if (newData && newData.user) {
    displayUsername.value = newData.user.username;
  } else {
    displayUsername.value = "Guest";
  }
}, { immediate: true }); 


const handleLogout = async () => {
  try {
    await signOut();
    await navigateTo('/Auth/login'); 
  } catch (error) {
    console.error("Logout failed:", error);
  }
};
</script>

<style scoped>
.navbar {
  background-color: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
  padding: 0.75rem 1.5rem;
  box-shadow: 0 1px 0 rgba(0, 0, 0, 0.08);
  border-bottom: 1px solid rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 100;
  border-bottom-left-radius: 0.5rem;
  border-bottom-right-radius: 0.5rem;
  font-family: "Inter", sans-serif; 
}
.navbar-container {
  max-width: 1280px;
  margin-left: auto;
  margin-right: auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

@media (min-width: 768px) {
  .navbar-container {
    flex-direction: row;
    gap: 0;
  }

  .user-info {
    order: 0;
  }

  .nav-links {
    flex-direction: row;
    gap: 2rem;
  }

  .logout-wrapper {
    order: 0;
  }
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.user-avatar {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 50%;
  border: 1px solid rgba(0, 0, 0, 0.1);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
  object-fit: cover; 
}

.username {
  color: #333;
  font-weight: 500;
  font-size: 1rem;
  transition: color 0.2s ease;
}

.username:hover {
  color: #007aff;
}

.nav-links {
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 0.75rem;
  flex-grow: 1;
  justify-content: center;
}

.nav-link {
  color: #333;
  font-size: 1rem;
  font-weight: 500;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  transition: background-color 0.2s ease, color 0.2s ease;
}

.nav-link:hover {
  background-color: rgba(0, 0, 0, 0.05);
  color: #000;
}

.logout-button {
  background-color: #f0f0f0;
  color: #333;
  font-weight: 500;
  font-size: 0.95rem;
  text-transform: none;
  padding: 0.6rem 1.25rem;
  border-radius: 20px;
  border: 1px solid rgba(0, 0, 0, 0.1);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
  transition: all 0.2s ease;
  cursor: pointer;
  outline: none;
}

.logout-button:hover {
  background-color: #e0e0e0;
  border-color: rgba(0, 0, 0, 0.15);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.logout-button:active {
  background-color: #d0d0d0;
  transform: translateY(1px);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.logout-button:focus {
  box-shadow: 0 0 0 2px rgba(0, 122, 255, 0.3);
}
</style>