import { ref, computed, onMounted } from 'vue'
import { defineStore } from 'pinia'

export const useMyProductStoreStore = defineStore('useProductStore', () => {
  const fakestore= ref([])
  const selectedCategory = ref('All')
  const searchTerm = ref('')
  const product = ref([])
  const filterVisible = ref(false)
  const categories = ref(['All', 'Clothing', 'Electronics'])
  const cart = ref([])
  const cartVisible = ref(false)
  const visible = ref(false);




  const fetchProducts = async () => {
    const { data: products, error } = await useFetch('https://fakestoreapi.com/products')
    if (!error.value && products.value) {
      fakestore.value = products.value.map(p => ({ ...p, quantity: 1 }))
    }
  }
  
const selectCategory = (category) => {
  selectedCategory.value = category
  filterVisible.value = false 
}
// Cart actions
const addToCart = (product) => {
  const existing = cart.value.find(p => p.id === product.id);
  if (existing) {
    existing.quantity++;
  } else {
    cart.value.push({ ...product, quantity: 1 });
  }
  cartVisible.value = true;
};

const removeProduct = (product) => {
  cart.value = cart.value.filter(p => p.id !== product.id);
  if (!cart.value.length) cartVisible.value = false;
};

const increaseQuantity = (product) => {
  product.quantity++;
};

const decreaseQuantity = (product) => {
  if (product.quantity > 1) product.quantity--;
  else removeProduct(product);
};

const cartTotal = computed(() =>
  cart.value.reduce((sum, i) => sum + i.price * i.quantity, 0).toFixed(2)
);



// Checkout
const checkout = () => {
  if (!cart.value.length) return alert('Cart is empty');
  alert(`Checking out ${cart.value.length} items - total $${cartTotal.value}`);
  cart.value = [];
  cartVisible.value = false;
};

  return {
    fakestore,
    selectedCategory,
    searchTerm,
    fetchProducts,
    product,
    selectCategory,
    categories,
    filterVisible,
    addToCart,
    cart,
    cartVisible,
    removeProduct,
    increaseQuantity,
    decreaseQuantity,
    cartTotal,
    checkout,
    visible
  }
})


