<template>
  <div>
    <hero/>
    <div
      class="page-wrapper"
      :class="{ 'filter-open': filterVisible, 'visible': visible, 'cart-open': cartVisible }"
    >
      <FilterSidebar
        :categories="categories"
        :selected="selectedCategory"
        :visible="filterVisible"
        @select="selectCategory"
        @toggle="filterVisible = !filterVisible"
      />

      <main class="content-area">
        <div class="toolbar">
          <button class="btn" @click="filterVisible = !filterVisible">
            {{ filterVisible ? 'Hide Filters' : 'Show Filters' }}
          </button>
        </div>

        <div class="listProduct">
          <ProductCard
            v-for="product in filteredProducts"
            :key="product.id"
            :product="product"
            @add-to-cart="addToCart"
            @remove-product="removeProduct"
            @view-product="viewProduct"
          />
        </div>
      </main>

      <CartSidebar
        :cart="cart"
        :visible="cartVisible"
        :total="cartTotal"
        @close-cart="cartVisible = false"
        @increase="increaseQuantity"
        @decrease="decreaseQuantity"
        @checkingout="checkout"
      />

    </div>
    <Thefooter />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useMyProductStoreStore } from '~/stores/productStore';

import ProductCard from '~/components/ProductCard.vue';
import FilterSidebar from '~/components/FilterSidebar.vue';
import CartSidebar from '~/components/CartSideBar.vue';
import Thefooter from '~/components/Thefooter.vue';
import hero from '~/components/hero-section.vue';

const myProductStore = useMyProductStoreStore();
const { fakestore, searchTerm, selectedCategory, filterVisible, categories, cart, cartVisible, cartTotal, visible} = storeToRefs(myProductStore);
const { fetchProducts, selectCategory, addToCart, removeProduct, increaseQuantity, decreaseQuantity, checkout} = myProductStore;

onMounted(() => {
  fetchProducts();
});

const filteredProducts = computed(() => {
  let prods = fakestore.value;

  if (selectedCategory.value !== 'All') {
    prods = prods.filter(
      p => p.category.toLowerCase() === selectedCategory.value.toLowerCase()
    );
  }

  if (searchTerm.value) {
    const lower = searchTerm.value.toLowerCase();
    prods = prods.filter(
      p =>
        p.title.toLowerCase().includes(lower) ||
        p.description.toLowerCase().includes(lower)
    );
  }

  return prods;
});


</script>

<style scoped>
.body{
  margin:0;
  padding:0;
}
.page-wrapper {
  display: flex;
  position: relative;
  transition: transform 0.3s ease;
  overflow: hidden;
  margin: 0;
}

.content-area {
  flex-grow: 1;
  padding: 20px;
  display: flex;
  flex-direction: column;
}

.toolbar {
  margin-bottom: 12px;
}

.btn {
  padding: 8px 12px;
  border: none;
  background: #007bff;
  color: white;
  border-radius: 4px;
  cursor: pointer;
}

.btn:hover {
  background: #0056b3;
}

.listProduct {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 16px;
}
</style>
