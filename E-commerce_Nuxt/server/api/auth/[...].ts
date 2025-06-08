import CredentialsProvider from 'next-auth/providers/credentials';
import { NuxtAuthHandler } from '#auth';

export default NuxtAuthHandler({
  secret: useRuntimeConfig().authSecret,

  providers: [
    CredentialsProvider.default({
      name: 'credentials',
      credentials: {
        email: { label: 'Email', type: 'email' },
        password: { label: 'Password', type: 'password' }
      },

      async authorize(credentials) {
        try {
          const runtimeConfig = useRuntimeConfig();
          const loginUrl = `${runtimeConfig.public.apiBaseUrl}/Auth/login`;

          const response = await $fetch(loginUrl, {
            method: 'POST',
            body: {
              email: credentials?.email,
              password: credentials?.password
            }
          });
          if (response && response.status === true) {
            return  {
              id: credentials?.email,
              email: credentials?.email,
              name: credentials?.email?.split('@')[0],
              accessToken: response.result.token,
              tokenExpires: response.result.expires
            };
          }
          return null;
        } catch (error) {
          if (error?.data?.status === false) {
            console.error('Login failed:', error.data.error);
          }
          return null;
        }
      }
    })

  ],
  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        token.accessToken = user.accessToken;
        token.tokenExpires = user.tokenExpires;
        token.id = user.id;
        token.email = user.email;
        token.name = user.name;
        if (user.tokenExpires) {
          token.exp = new Date(user.tokenExpires).getTime() / 1000;
        }
      } 
      return token;
    },

    async session({ session, token }) {
      if (token) {
        session.accessToken = token.accessToken;
        session.user.id = token.id;
        session.user.email = token.email;
        session.user.name = token.name;
        session.tokenExpires = token.tokenExpires;
      }
      return session;
    }
  },

  pages: {
    signIn: '/Auth/login',
    error: '/Auth/error'
  },

  session: {
    strategy: 'jwt',
    maxAge: 24 * 60 * 60,
  }
});
