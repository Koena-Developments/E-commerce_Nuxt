import NextAuth, { DefaultSession } from "next-auth";
import { JWT } from "next-auth/jwt";

declare module "next-auth" {
  
  interface Session {
    accessToken?: string;
    tokenExpires?: string; 
    user: {
      id?: string;
      email?: string;
      name?: string;
      
    } & DefaultSession["user"]; 
  }

  
  interface User {
    accessToken?: string;
    tokenExpires?: string; 
  }
}

declare module "next-auth/jwt" {

  interface JWT {
    accessToken?: string;
    tokenExpires?: string;
    id?: string;
    email?: string;
    name?: string;
    exp?: number; 
  }
}