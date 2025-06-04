<template>
  <section class="hero-section">
    <div class="hero-content">
      <h1>Discover Your Next Favorite Product</h1>
      <p>Explore a wide range of products from various categories.</p>
      <div class="search-bar-container">
        <input
          type="text"
          placeholder="Search products..."
          :value="modelValue"
          @input="$emit('update:modelValue', $event.target.value)"
          class="search-input"
        />
        <button class="search-button">Search</button>
      </div>
    </div>
  </section>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue';

defineProps({
  modelValue: String,
});

defineEmits(['update:modelValue']);
</script>

<style scoped>
.hero-section {
  background-image: url('https://images.unsplash.com/photo-1497215729119-ed2090687dc6?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D');
  background-size: cover;
  background-position: center;
  color: white;
  text-align: center;
  padding: 80px 20px;
  min-height: 350px;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative; 
}

.hero-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5); 
  z-index: 0;
}

.hero-content {
  position: relative;
  z-index: 1;
  max-width: 800px;
  width: 100%;
}

.hero-content h1 {
  font-size: 3.5em;
  margin-bottom: 15px;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.7);
}

.hero-content p {
  font-size: 1.5em;
  margin-bottom: 30px;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.7);
}

.search-bar-container {
  display: flex;
  justify-content: center;
  width: 100%;
  max-width: 600px;
  margin: 0 auto;
  border-radius: 50px;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
}

.search-input {
  flex-grow: 1;
  padding: 15px 25px;
  border: none;
  font-size: 1.1em;
  outline: none;
  border-radius: 50px 0 0 50px;
}

.search-button {
  padding: 15px 30px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 0 50px 50px 0;
  cursor: pointer;
  font-size: 1.1em;
  transition: background-color 0.3s ease;
}

.search-button:hover {
  background-color: #0056b3;
}

@media (max-width: 768px) {
  .hero-content h1 {
    font-size: 2.5em;
  }
  .hero-content p {
    font-size: 1.2em;
  }
  .search-input, .search-button {
    font-size: 1em;
    padding: 12px 20px;
  }
  .search-bar-container {
    flex-direction: column;
    border-radius: 10px;
  }
  .search-input {
    border-radius: 10px 10px 0 0;
    margin-bottom: 1px; 
  }
  .search-button {
    border-radius: 0 0 10px 10px;
  }
}
</style>