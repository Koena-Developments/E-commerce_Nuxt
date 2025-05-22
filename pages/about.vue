<template>
  <section class="about-page">
    <div class="hero">
      <h1>About Us</h1>
      <p>
        We’re passionate about building beautiful, user-centric e-commerce experiences.
        Our mission is to bring top-quality products right to your doorstep—seamlessly.
      </p>
    </div>

    <div class="story">
      <h2>Our Story</h2>
      <p>
        Founded in 2022 by a group of web-obsessed friends, we started with a simple idea:
        make online shopping joyful and reliable. Since then, we've grown into a
        small but mighty team of designers, developers, and dreamers.
      </p>
    </div>

    <div class="team">
      <h2>Meet the Team</h2>
      <div class="team-grid">
        <div class="team-member" v-for="member in team" :key="member.id">
          <!-- <img :src="member.photo" :alt="member.name" /> -->
          <h3>{{ member.name }}</h3>
          <p>{{ member.role }}</p>
        </div>
      </div>
    </div>

    <div class="cta">
      <p>Want to work with us or learn more?</p>
      <button @click="goContact">Contact Us</button>
    </div>
  </section>
</template>

<script setup>
import { useRouter } from 'vue-router'

definePageMeta({
  layout: 'products'
})

const team = [
  { id: 1, name: 'Trent FisherMan', role: 'Lead Developer', photo: '/team/trent.jpg' },
  { id: 2, name: 'Taylon Cordae', role: 'Head of Design', photo: '/team/Tay.jpg' },
  { id: 3, name: 'Thabang Motswenyane', role: 'The Intern', photo: '/team/Thabang.jpg' },
  { id: 4, name: 'Michael Bolton', role: 'Marketing Director', photo: '/team/pluto.jpg' }
]

const router = useRouter()
const goContact = () => router.push('/')
</script>

<style scoped>
.about-page {
  max-width: 900px;
  margin: 0 auto;
  padding: 40px 20px;
  line-height: 1.6;
}

.hero {
  text-align: center;
  margin-bottom: 60px;
}
.hero h1 {
  font-size: 3rem;
  margin-bottom: 20px;
}
.hero p {
  font-size: 1.125rem;
  color: #555;
}

.story {
  margin-bottom: 60px;
}
.story h2 {
  font-size: 2rem;
  margin-bottom: 16px;
}
.story p {
  color: #444;
}

.team {
  margin-bottom: 60px;
}
.team h2 {
  font-size: 2rem;
  text-align: center;
  margin-bottom: 32px;
}
.team-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
  gap: 24px;
}
.team-member {
  text-align: center;
}
.team-member img {
  width: 120px;
  height: 120px;
  object-fit: cover;
  border-radius: 50%;
  border: 2px solid #eee;
  margin-bottom: 12px;
}
.team-member h3 {
  font-size: 1.125rem;
  margin-bottom: 4px;
}
.team-member p {
  font-size: 0.95rem;
  color: #777;
}

.cta {
  text-align: center;
  padding: 40px 0;
  border-top: 1px solid #eee;
}
.cta p {
  font-size: 1.125rem;
  margin-bottom: 16px;
}
.cta button {
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 12px 24px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.2s ease;
}
.cta button:hover {
  background-color: #0056b3;
}

@media (max-width: 600px) {
  .hero h1 { font-size: 2.4rem; }
  .story h2,
  .team h2 { font-size: 1.75rem; }
}
</style>
