import { ref, computed, onMounted } from 'vue'
import { defineStore } from 'pinia'

export const useMyProductStore = defineStore('useProductStore', () => {

 interface Product{
 id: Number;
  title: string;
  description: string;
  category: string;
  price: number;
  image?: string;
  quantity: number;
 } 


  const fakestore= ref(<Product[]>[])
  const selectedCategory = ref('All')
  const searchTerm = ref('')
  const product = ref(<Product[]>[])
  const filterVisible = ref(false)
  const categories = ref(['All', 'Clothing', 'Electronics'])
  const cart = ref<Product[]>([])
  const cartVisible = ref(false)
  const visible = ref(false);

  const fetchProducts = async () => {
    const { data: products, error } = await useFetch<Product[]>('https://fakestoreapi.com/products')
    if (!error.value && products.value) {
      fakestore.value = products.value.map(p => ({ ...p, quantity: 1 }))
    }
  }
  
 const selectCategory = (category: string) => {
    selectedCategory.value = category
    filterVisible.value = false
  }
// Cart actions
const addToCart = (product: Product) => {
  const existing = cart.value.find(p => p.id === product.id);
  if (existing) {
    alert('Product already in cart');
    return;
  }
  cart.value.push({ ...product, quantity: 1 });
  cartVisible.value = true;
};

const removeProduct = (product: Product) => {
  cart.value = cart.value.filter(p => p.id !== product.id);
  if (!cart.value.length) cartVisible.value = false;
};

const increaseQuantity = (product: Product) => {
  product.quantity++;
};



const decreaseQuantity = (product: Product) => {
  if (product.quantity > 1) 
    product.quantity--;
  else removeProduct(product);
};

const cartTotal = computed(() =>
  cart.value.reduce((sum, i) => sum + i.price * i.quantity, 0).toFixed(2)
);

// Checkout
const checkout = () => {
  if (!cart.value.length) {
    return alert('Cart is empty')
  }
  const paymentLinks:Record<string, string>  = {
      1:  'https://buy.stripe.com/test_eVqaEZ6L6bgi5wMgOD1wY01', 
      2:  'https://buy.stripe.com/test_6oUeVfedy702aR655V1wY02',
      3:  'https://buy.stripe.com/test_6oU5kFglG3NQ2kA8i71wY03', 
      4:  'https://buy.stripe.com/test_dRm3cx3yU7023oEbuj1wY04',
      5:  'https://buy.stripe.com/test_cNi4gB6L60BEf7mgOD1wY05',
      6:  'https://buy.stripe.com/test_3cI5kF3yUace3oE55V1wY07',
      7:  'https://buy.stripe.com/test_00w9AVc5q8465wMcyn1wY06',
      8:  'https://buy.stripe.com/test_9B6aEZ1qM846aR6eGv1wY08',
      9:  'https://buy.stripe.com/test_28EbJ3d9udoqgbqaqf1wY09',
      10: 'https://buy.stripe.com/test_cNieVfb1m3NQcZe41R1wY0a',
      11: 'https://buy.stripe.com/test_fZu3cxb1m3NQ2kAeGv1wY0b',
      12: 'https://buy.stripe.com/test_6oU00lb1mgAC7EUgOD1wY0d',
      13: 'https://buy.stripe.com/test_7sY28t6L60BE6AQfKz1wY0e',
      14: 'https://buy.stripe.com/test_dRm28t9Xi2JM6AQdCr1wY0f',
      15: 'https://buy.stripe.com/test_7sY4gBd9u2JMaR669Z1wY0g',
      16: 'https://buy.stripe.com/test_eVq7sN8Teesu9N2gOD1wY0h',
      17: 'https://buy.stripe.com/test_8x2aEZ7Pa8461gw55V1wY0i',
      18: 'https://buy.stripe.com/test_4gM6oJ5H21FI1gweGv1wY0j',
      19: 'https://buy.stripe.com/test_00w3cx1qMgAC3oEdCr1wY0k',
      20: 'https://buy.stripe.com/test_fZubJ36L698acZe69Z1wY0l'
    };
  const link = paymentLinks[Number(cart.value[0].id)]
  // if (link) {
  //   window.location.href = link
  // } else {
  //   alert(`Checked out ${cart.value.length} items totaling R${cartTotal.value}`)
  //   // clearCart()
  // }
}

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
    visible,
  }
})


