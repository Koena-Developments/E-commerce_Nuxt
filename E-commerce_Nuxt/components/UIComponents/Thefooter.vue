<template>
  <footer class="site-footer">
    <div class="footer-container">

      <div class="footer-col">
        <h3 class="footer-logo">Products World</h3>
        <p class="footer-tagline">
          Hand-picked products delivered to your door.
        </p>
      </div>

      <div class="footer-col">
        <h4>Shop</h4>
        <ul>
          <li><NuxtLink to="/">Home</NuxtLink></li>
          <li><NuxtLink to="/products">All Products</NuxtLink></li>
          <li><NuxtLink to="/about">About Us</NuxtLink></li>
          <li><NuxtLink to="/contact">Contact</NuxtLink></li>
        </ul>
      </div>

      <div class="footer-col">
        <h4>Support</h4>
        <ul>
          <li><NuxtLink to="/">Home</NuxtLink></li>
          <li><NuxtLink to="/products">All Products</NuxtLink></li>
          <li><NuxtLink to="/about">About Us</NuxtLink></li>
          <li><NuxtLink to="/contact">Contact</NuxtLink></li>
        </ul>
      </div>

      <div class="footer-col">
        <h4>Follow Us</h4>
        <div class="social-icons">
          <a href="#" aria-label="Facebook"><i class="fab fa-facebook-f"></i></a>
          <a href="#" aria-label="Twitter"><i class="fab fa-twitter"></i></a>
          <a href="#" aria-label="Instagram"><i class="fab fa-instagram"></i></a>
          <a href="#" aria-label="LinkedIn"><i class="fab fa-linkedin-in"></i></a>
        </div>
      </div>
    </div>
    <div class="footer-bottom">
      <p>© {{ new Date().getFullYear() }} Products World. All rights reserved.</p>
    </div>
  </footer>
</template>

<script setup>
</script>

<style scoped>
.site-footer {
  background: #222;
  color: #eee;
  padding: 40px 20px 20px;
  font-size: 0.9rem;
}
.footer-container {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 24px;
  max-width: 1200px;
  margin: 0 auto;
}
.footer-col h4,
.footer-col .footer-logo {
  margin-bottom: 12px;
  color: #fff;
}
.footer-logo {
  font-size: 1.5rem;
  font-weight: bold;
}
.footer-tagline {
  font-size: 0.95rem;
  line-height: 1.4;
}
.footer-col ul {
  list-style: none;
  padding: 0;
}
.footer-col li {
  margin-bottom: 8px;
}
.footer-col a {
  color: #ccc;
  text-decoration: none;
  transition: color 0.2s ease;
}
.footer-col a:hover {
  color: #fff;
}
.social-icons a {
  display: inline-block;
  margin-right: 12px;
  font-size: 1.2rem;
  color: #ccc;
  transition: color 0.2s ease;
}
.social-icons a:hover {
  color: #fff;
}
.footer-bottom {
  text-align: center;
  margin-top: 24px;
  border-top: 1px solid #444;
  padding-top: 12px;
  font-size: 0.85rem;
  color: #888;
}
</style>
