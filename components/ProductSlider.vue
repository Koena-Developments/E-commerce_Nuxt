<template>
  <div class="slider-wrapper">
    <button class="arrow left" @click="scroll(-1)">‹</button>
    <div ref="track" class="slider-track">
      <ProductCard
        v-for="product in products"
        :key="product.id"
        :product="product"
      />
    </div>
    <button class="arrow right" @click="scroll(1)">›</button>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import ProductCard from '~/components/ProductCard.vue'

defineProps({
  products: {
    type: Array,
    required: true
  }
})

const track = ref(null)
const scroll = (dir) => {
  if (!track.value) return
  const width = track.value.clientWidth
  track.value.scrollBy({ left: dir * (width * 0.8), behavior: 'smooth' })
}
</script>

<style scoped>
.slider-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  margin: 20px 0;
}
.slider-track {
  display: flex;
  overflow-x: auto;
  scroll-behavior: smooth;
  gap: 16px;
  padding: 0 40px; /* space for arrows */
  scrollbar-width: none;
}
/* hide scrollbar */
.slider-track::-webkit-scrollbar { display: none; }

.arrow {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  z-index: 10;
  background: rgba(255,255,255,0.8);
  border: none;
  font-size: 2rem;
  line-height: 1;
  width: 32px;
  height: 56px;
  cursor: pointer;
  border-radius: 4px;
}
.arrow.left  { left: 8px;  }
.arrow.right { right: 8px; }
.arrow:hover {
  background: rgba(0, 123, 255, 0.8);
  color: white;
}
</style>
