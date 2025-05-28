import {NuxtAuthHandler} from '#auth'
import GithubProvider from 'next-auth/providers/github'


export default NuxtAuthHandler({
  providers: [
    // @ts-expect-error You need to use .default here for it to work correctly
    GithubProvider.default({
      clientId: 'Ov23liD8gMOiszU87jyT',
      clientSecret: 'adle381b3ad63a2d3e85f73f192b69c0d441721856dkhf',
    }),
  ],
});