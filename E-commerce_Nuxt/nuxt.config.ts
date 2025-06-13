import { defineNuxtConfig } from 'nuxt/config';
export default defineNuxtConfig({
  compatibilityDate: '2025-05-15',
  devtools: { enabled: true },
  css: [
    '@fortawesome/fontawesome-free/css/all.min.css', 
    '~/assets/css/main.css'
  ],
  modules: [
    '@nuxt/fonts',
    '@pinia/nuxt',
    '@sidebase/nuxt-auth'
  ],

  auth: {
    baseURL: process.env.AUTH_ORIGIN, 
    globalAppMiddleware: {
      isEnabled: true
    },
    provider: {
      type: 'authjs'
    }
  },

  runtimeConfig: {
    authSecret: process.env.NUXT_AUTH_SECRET,
    stripeSecretKey: process.env.STRIPE_SECRET_KEY,
    public: {
      stripePublishableKey: process.env.NUXT_PUBLIC_STRIPE_PUBLISHABLE_KEY,
      apiBaseUrl: process.env.NUXT_PUBLIC_API_BASE_URL
    }
  }
});