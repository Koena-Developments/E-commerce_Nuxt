// import CredentialsProvider from '@auth/credentials-provider'
// import { NuxtAuthHandler } from '#auth'

// export default NuxtAuthHandler({
//   secret: useRuntimeConfig().authSecret,
  
//   providers: [
//     CredentialsProvider({
//       name: 'credentials',
//       credentials: {
//         email: { label: 'Email', type: 'email' },
//         password: { label: 'Password', type: 'password' }
//       },
      
//       async authorize(credentials) {
//         try {
//           const response = await $fetch('/auth/login', {
//             method: 'POST',
//             baseURL: useRuntimeConfig().apiBaseUrl,
//             body: {
//               email: credentials?.email,
//               password: credentials?.password
//             }
//           })

//           if (response && response.status === true && response.result) {
//             return {
//               id: credentials?.email, 
//               email: credentials?.email,
//               name: credentials?.email?.split('@')[0], 
//               accessToken: response.result.token,
//               tokenExpires: response.result.expires
//             }
//           }
          

//           return null
//         } catch (error) {
//           console.error('Authentication error:', error)
          
    
//           if (error?.data?.status === false) {
//             console.error('Login failed:', error.data.error)
//           }
          
//           return null
//         }
//       }
//     })
//   ],

//   callbacks: {
    
//     async jwt({ token, user }) {
    
//       if (user) {
//         token.accessToken = user.accessToken
//         token.tokenExpires = user.tokenExpires
//         token.id = user.id
//         token.email = user.email
//         token.name = user.name
//       }
      
//       if (token.tokenExpires && new Date() > new Date(token.tokenExpires)) {
//         return null
//       }
      
//       return token
//     },
    
//     async session({ session, token }) {
//       if (token) {
//         session.accessToken = token.accessToken
//         session.user.id = token.id
//         session.user.email = token.email
//         session.user.name = token.name
//         session.tokenExpires = token.tokenExpires
//       }
//       return session
//     }
//   },

//   pages: {
//     signIn: '/login',
//     error: '/auth/error'
//   },

//   session: {
//     strategy: 'jwt',
//     maxAge: 24 * 60 * 60,
//   }
// })