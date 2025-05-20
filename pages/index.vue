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
</template>



<script setup>
import {ref, computed, onMounted} from 'vue'



const fakestore = ref([])
const cart = ref([])
const searchTerm = ref('')
const counter = ref(0)


const filteredProducts = computed(() => {
    const term = searchTerm.value.toLowerCase()
    return fakestore.value.filter(product => product.title.toLowerCase().includes(term))
})

// the key is data and the val will be stored in products

//fetching our data and sharing its data into the fakestore array
const fetchProducts = async () => {
    const { data: products } = await useFetch('https://fakestoreapi.com/products')
    fakestore.value = products.value 
}

const addToCart=(product)=>{
    if(product.id in cart){
        counter.value +=1
        cart.value.push(product)
    }
}
const removeProduct =(product) =>{
    const index = cart.value.indexOf(product)
    if(index > -1){
    cart.value.split(index,1)
    }
}

onMounted(() => {
    fetchProducts()
})
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