// import { defineStore } from 'pinia';
// import { ref, computed, watch } from 'vue';
// import { useAuth, useRuntimeConfig, navigateTo } from '#imports';

// export const useUserStore = defineStore('user', () => {
//   const profile = ref(null); 
//   const profile = ref<any | null>(null); 
//   const isLoadingProfile = ref(false);
//   const profileError = ref<string | null>(null);
//   const preferences = ref<{ darkMode: boolean; language: string }>({ darkMode: false, language: 'en' });

//   const { status, data: session, getSession, signIn, signOut } = useAuth();
//   const runtimeConfig = useRuntimeConfig(); 

//   const isAuthenticated = computed(() => status.value === 'authenticated');
//   const userId = computed(() => session.value?.user?.id);
//   const userToken = computed(() => session.value?.accessToken); 
//   type ApiResponse<T> = {
//     status: boolean;
//     result?: T;
//     error?: string;
//   };

//   async function fetchUserProfile() {
//     if (!isAuthenticated.value || !userId.value || !userToken.value) {
//       profile.value = null; 
//       return;
//     }

//     isLoadingProfile.value = true;
//     profileError.value = null;

//     try {
//       const response = await $fetch<ApiResponse<any>>('/api/User/profile', {
//         method: 'GET',
//         baseURL: runtimeConfig.public.apiBaseUrl,
//         headers: {
//           'Content-Type': 'application/json',
//           'Authorization': `Bearer ${userToken.value}`,
//         },
//       });

//       if (response && response.status === true && response.result) {
//         profile.value = response.result;
//       } else {
//         profileError.value = response?.error || 'Failed to fetch user profile.';
//       }
//     } catch (error: unknown) {
//       let message = 'Unknown error';
//       let statusCode: number | undefined;
//       if (typeof error === 'object' && error !== null) {
//   async function updateUserProfile(updatePayload: Record<string, any>): Promise<[boolean, string | null]> {
//     if (!isAuthenticated.value || !userId.value || !userToken.value) {
//       profileError.value = 'Not authenticated to update profile.' as string;
//       return [false, profileError.value];
//     }

//     isLoadingProfile.value = true;
//     profileError.value = null;

//     try {
//       const response = await $fetch<ApiResponse<any>>(`/api/User/profile/${userId.value}`, {
//         method: 'PUT',
//         baseURL: runtimeConfig.public.apiBaseUrl,
//         headers: {
//           'Content-Type': 'application/json',
//           'Authorization': `Bearer ${userToken.value}`,
//         },
//         body: updatePayload,
//       });

//       if (response && response.status === true) {
//         if (response.result) {
//             profile.value = response.result;
//         }
//         if (typeof getSession === 'function') {
//           await getSession();
//         }
//         return [true, null]; 
//       } else {
//         profileError.value = response?.error || 'Failed to update profile.';
//         return [false, profileError.value]; 
//       }
//     } catch (error: unknown) {
//       let message = 'Unknown error';
//       let statusCode: number | undefined;
//       if (typeof error === 'object' && error !== null) {
//         message = (error as any).message || message;
//         statusCode = (error as any).statusCode;
//       }
//       profileError.value = `Error updating profile: ${statusCode || message}`;
//       console.error('User Store - Update Profile Error:', error);
//       if (statusCode === 401 || statusCode === 403) {
//          navigateTo('/Auth/login');
//       }
//       return [false, profileError.value]; 
//     } finally {
//       isLoadingProfile.value = false;
//     }
//   }
//         return (false, profileError.value); 
//       }
//     } catch (error) {
//       profileError.value = `Error updating profile: ${error.statusCode || error.message}`;
//       console.error('User Store - Update Profile Error:', error);
//       if (error.statusCode === 401 || error.statusCode === 403) {
//          navigateTo('/Auth/login');
//       }
//       return (false, profileError.value); 
//     } finally {
//       isLoadingProfile.value = false;
//     }
//   }

//   watch(isAuthenticated, (newStatus) => {
//     if (newStatus) {
//       fetchUserProfile(); 
//     } else {
//       profile.value = null; 
//       preferences.value = { darkMode: false, language: 'en' }; 
//       profileError.value = null;
//     }
//   }, { immediate: true }); 

//   return {
//     profile,
//     preferences,
//     isLoadingProfile,
//     profileError,
//     isAuthenticated,
//     userId,
//     fetchUserProfile,
//     updateUserProfile,
//   };
// });