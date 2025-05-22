<template>
  <div class="container">
    <div class="slide">
      <div
        v-for="product in products"
        :key="product.id"
        class="item"
        :style="{ backgroundImage: 'url(' + product.image + ')' }"
      >
        <div class="content">
          <div class="name">{{ product.title }}</div>
          <div class="description">
            <p>{{ product.description }}</p>
          </div>
          <button>See More</button>
        </div>
      </div>
    </div>
    <div class="button-controls">
      <button class="prev">-</button>
      <button class="next">_</button>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';

const props = defineProps({
  products: {
    type: Array,
    required: true,
  }
});

onMounted(() => {
  const nextButton = document.querySelector('.next');
  const prevButton = document.querySelector('.prev');
  const slideContainer = document.querySelector('.slide');

  if (nextButton && slideContainer) {
    nextButton.addEventListener('click', () => {
      const items = document.querySelectorAll('.item');
      if (items.length > 0) {
        slideContainer.appendChild(items[0]);
      }
    });
  }

  if (prevButton && slideContainer) {
    prevButton.addEventListener('click', () => {
      const items = document.querySelectorAll('.item');
      if (items.length > 0) {
        slideContainer.prepend(items[items.length - 1]);
      }
    });
  }
});
</script>

<style scoped>
.container {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 90vw;
  max-width: 1000px;
  height: 600px;
  background: #f5f5f5;
  box-shadow: 0 30px 50px #dbdbdb;
  overflow: hidden;
  border-radius: 20px;
}

.container .slide {
  position: relative;
  width: 100%;
  height: 100%;
}

.container .slide .item {
  width: 200px;
  height: 300px;
  position: absolute;
  top: 50%;
  transform: translate(0, -50%);
  border-radius: 20px;
  box-shadow: 0 30px 50px #dbdbdb;
  background-position: 50% 50%;
  background-size: cover;
  transition: 0.5s;
  overflow: hidden;
}

.slide .item:nth-child(1) {
  top: 0;
  left: 0;
  transform: translate(0, 0);
  border-radius: 0;
  width: 100%;
  height: 100%;
}

.slide .item:nth-child(2) {
  top: 0;
  left: 0;
  transform: translate(0, 0);
  border-radius: 0;
  width: 100%;
  height: 100%;
}

.slide .item:nth-child(3) {
  left: 50%;
}

.slide .item:nth-child(4) {
  left: calc(50% + 220px);
}

.slide .item:nth-child(5) {
  left: calc(50% + 440px);
}

.slide .item:nth-child(n + 6) {
  left: calc(50% + 660px);
  opacity: 0;
  pointer-events: none;
}

.item .content {
  position: absolute;
  top: 50%;
  left: 100px;
  width: 300px;
  text-align: left;
  color: #eee;
  transform: translate(0, -50%);
  font-family: 'Inter', system-ui, sans-serif;
  display: none;
  background-color: rgba(0, 0, 0, 0.5);
  padding: 20px;
  border-radius: 10px;
  z-index: 2;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
}

.slide .item:nth-child(2) .content {
  display: block;
}

.content .name {
  font-size: 40px;
  text-transform: uppercase;
  font-weight: bold;
  opacity: 0;
  animation: animate 1s ease-in-out 0.3s 1 forwards;
  margin-bottom: 10px;
}

.content .description {
  margin-top: 10px;
  margin-bottom: 20px;
  font-size: 0.9em;
  line-height: 1.5;
}

.content button {
  padding: 12px 25px;
  border: none;
  cursor: pointer;
  opacity: 0;
  animation: animate 1s ease-in-out 0.6s 1 forwards;
  background-color: #007bff;
  color: white;
  border-radius: 8px;
  transition: background-color 0.3s ease, transform 0.3s ease;
  font-weight: bold;
}

.content button:hover {
  background-color: #0056b3;
  transform: translateY(-2px);
}

.button-controls {
  width: 100%;
  text-align: center;
  position: absolute;
  bottom: 20px;
  z-index: 10;
}

.button-controls button {
  width: 45px;
  height: 45px;
  border-radius: 50%;
  border: 1px solid #000;
  cursor: pointer;
  margin: 0 10px;
  background-color: #fff;
  color: #000;
  font-size: 1.5em;
  font-weight: bold;
  transition: 0.3s ease;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
}

.button-controls button:hover {
  background: #ababab;
  color: #fff;
  transform: translateY(-3px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.3);
}

@keyframes animate {
  from {
    opacity: 0;
    transform: translate(0, 100px);
    filter: blur(33px);
  }
  to {
    opacity: 1;
    transform: translate(0);
    filter: blur(0);
  }
}

@media (max-width: 768px) {
  .container {
    height: 500px;
    width: 95vw;
  }

  .item .content {
    left: 50%;
    transform: translate(-50%, -50%);
    width: 80%;
    text-align: center;
    padding: 15px;
  }

  .content .name {
    font-size: 30px;
  }

  .content .description {
    font-size: 0.8em;
  }

  .content button {
    padding: 10px 20px;
  }

  .slide .item:nth-child(3) {
    left: calc(50% - 110px);
  }
  .slide .item:nth-child(4) {
    left: calc(50% + 110px);
  }
  .slide .item:nth-child(5) {
    left: calc(50% + 330px);
  }
  .slide .item:nth-child(n + 6) {
    left: calc(50% + 550px);
  }
}

@media (max-width: 480px) {
  .container {
    height: 400px;
  }

  .item .content {
    width: 90%;
    left: 50%;
    transform: translate(-50%, -50%);
    padding: 10px;
  }

  .content .name {
    font-size: 24px;
  }

  .content .description {
    font-size: 0.7em;
    margin-bottom: 10px;
  }

  .content button {
    padding: 8px 15px;
    font-size: 0.9em;
  }

  .button-controls button {
    width: 35px;
    height: 35px;
    font-size: 1.2em;
    margin: 0 5px;
  }

  .slide .item:nth-child(3) {
    left: calc(50% - 80px);
  }
  .slide .item:nth-child(4) {
    left: calc(50% + 80px);
  }
  .slide .item:nth-child(5) {
    left: calc(50% + 240px);
  }
  .slide .item:nth-child(n + 6) {
    left: calc(50% + 400px);
  }
}
</style>
