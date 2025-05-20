<template>
  <div class="item">
    <img :src="product.image" :alt="product.title" />
    
    <div class="details">
    
    <div class="title">
    <h2>{{ product.title }}</h2>
    </div>
    
    <div class="price">R{{ product.price.toFixed(2) }}
    </div>
    
    <div class="btns">
     <button @click="$emit('add-to-cart', product)">Add to Cart</button>
     <button @click="$emit('remove-product', product)">Remove</button> 
    </div>
    
    <div>
    <router-link :to="`/products/${product.id}`"><button>View Product</button></router-link>
    </div>  
  </div>
  
  </div>
</template>

<script setup>
import {defineProps, defineEmits} from 'vue'

defineProps({
  product: {
    type: Object,
    required: true
  }
});
defineEmits(['add-to-cart', 'remove-product']);
</script>

<style scoped>
.item {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  background: #EEEEE6;
  padding: 20px;
  border-radius: 20px;
  filter: drop-shadow(0 15px 20px #0009);
  width: 200px;
  height: 200px;
  cursor: pointer;
  transition: .5s ease-in;
}

.details
{
  position: absolute;
  bottom: 0;
  overflow: hidden;
  width: 100%;
  height: 0;
  font-size: 12px;
  transition: .5s ease;
}

.item .title
{
  font-size: 8px;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  width: 100%;
  height: 86px;
}
.item img {
  width: 150px;
  height: 150px;
  object-fit: contain;
}


.item h2 {
  font-weight: 500;
  font-size: large;
}

.item .price {
  letter-spacing: 7px;
  font-size: 18px;
}
.item button {
  background: #353432;
  color: #eee;
  border: none;
  padding: 8px 10px;
  margin-top: 10px;
  border-radius: 20px;
  margin-left: 4px;
  cursor: pointer;
}

.item:hover .details {
  height: 85%;
  color: white;
}
.item:hover
{
  background: black;
  transform: scale(1.1);
}
.item img:hover {
  width: 150px;
  height: 150px;
  object-fit: contain;
} 
</style>