export default defineNuxtConfig({
  compatibilityDate: '2025-05-15',
  devtools: { enabled: true },
  css: [
    '@fortawesome/fontawesome-free/css/all.min.css'
  ],
  modules: ['@nuxt/fonts', '@pinia/nuxt', '@sidebase/nuxt-auth'],
  // auth: {
  //  baseURL: process.env.AUTH_ORIGIN, 
  // provider: {
  //   type: 'authjs'
  // }
  // },
  //   runtimeConfig: {
  //   authSecret: process.env.NUXT_AUTH_SECRET,
  //   public: {
  //     apiBaseUrl: process.env.NUXT_API_BASE_URL,
  //     authUrl: process.env.AUTH_ORIGIN
  //   }
  // }

// '@sidebase/nuxt-auth'
})