import { defineStore } from 'pinia';
import { ref, computed, watch } from 'vue';
import { useAuth } from '#app'; 
import { useRuntimeConfig } from '#app';

export const useUserStore = defineStore('user', () => {
  const profile = ref(null); 
  const isLoadingProfile = ref(false);
  const profileError = ref(null);

  const { data: authData, status } = useAuth();
  const runtimeConfig = useRuntimeConfig(); 

  const isAuthenticated = computed(() => status.value === 'authenticated');
  const userId = computed(() => authData.value?.user?.id);
  const userToken = computed(() => authData.value?.accessToken); 

  async function fetchUserProfile() {
    if (!isAuthenticated.value || !userId.value || !userToken.value) {
      profile.value = null; 
    }

    isLoadingProfile.value = true;
    profileError.value = null;

    try {
      const response = await $fetch('/api/User/profile', {
        method: 'GET',
        baseURL: runtimeConfig.public.apiBaseUrl,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${userToken.value}`,
        },
      });

      if (response && response.status === true && response.result) {
        profile.value = response.result;
      } else {
        profileError.value = response?.error || 'Failed to fetch user profile.';
      }
    } catch (error) {
      profileError.value = `Error fetching profile: ${error.statusCode || error.message}`;
      console.error('User Store - Fetch Profile Error:', error);
      if (error.statusCode === 401 || error.statusCode === 403) {
        navigateTo('/Auth/login');
      }
    } finally {
      isLoadingProfile.value = false;
    }
  }

  async function updateUserProfile(updatePayload) {
    if (!isAuthenticated.value || !userId.value || !userToken.value) {
      profileError.value = 'Not authenticated to update profile.';
      return (false, profileError.value);
    }

    isLoadingProfile.value = true;
    profileError.value = null;

    try {
      const response = await $fetch(`/api/User/profile/${userId.value}`, {
        method: 'PUT',
        baseURL: runtimeConfig.public.apiBaseUrl,
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${userToken.value}`,
        },
        body: updatePayload,
      });

      if (response && response.status === true) {
        if (response.result) {
            profile.value = response.result;
        }
        await authData.value.refresh();
        return (true, null); 
      } else {
        profileError.value = response?.error || 'Failed to update profile.';
        return (false, profileError.value); 
      }
    } catch (error) {
      profileError.value = `Error updating profile: ${error.statusCode || error.message}`;
      console.error('User Store - Update Profile Error:', error);
      if (error.statusCode === 401 || error.statusCode === 403) {
         navigateTo('/Auth/login');
      }
      return (false, profileError.value); 
    } finally {
      isLoadingProfile.value = false;
    }
  }

  watch(isAuthenticated, (newStatus) => {
    if (newStatus) {
      fetchUserProfile(); 
    } else {
      profile.value = null; 
      preferences.value = { darkMode: false, language: 'en' }; 
      profileError.value = null;
    }
  }, { immediate: true }); 

  return {
    profile,
    preferences,
    isLoadingProfile,
    profileError,
    isAuthenticated,
    userId,
    fetchUserProfile,
    updateUserProfile,
  };
});