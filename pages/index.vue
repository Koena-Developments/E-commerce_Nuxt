<template>
<div class="listProduct">
      <ProductCard
        v-for="product in filteredProducts"
        :key="product.id"
        :product="product"
        @add-to-cart="addToCart"
        @remove-product="removeProduct"
      />
    </div>

    <!-- <div>
       <div class="product-view">
        <div v-for="product in products" :key="product.id">   
            <NuxtLink :to="`/products/${product.id}`">{{ product.title}}</NuxtLink>
            <img :src="product.image" alt="{{ product.title}}">
        </div>
       </div>
    </div> -->
</template>



<script setup>
import {ref, computed} from 'vue'
definePageMeta({
    layout: 'default'
})


const fakestore = ref([])
const cart = ref([])
const total = ref(0)
const searchTerm = ref('')


const filteredProducts = computed(() => {
    const term = searchTerm.value.toLowerCase()
    return fakestore.value.filter(product => product.title.toLowerCase().includes(term))
})


// the key is data and the val will be stored in products
 const {data: products} =  await useFetch('https://fakestoreapi.com/products')
fakestore.value = products.value

const addToCart=(product)=>{
    cart.value.push(product)
}

const removeProduct=(product)=>{
    cart.value.pop(product.id)
}
</script>

<style scoped>
h2{
    margin-bottom: 20px;
    font-size: 36px;
}

p{
    margin: 20px 0;
}

.listProduct {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 10px;
}
</style>