const menu = [
  { id: 1, name: 'Cerveja Artesanal', price: 12.0 },
  { id: 2, name: 'Vinho Tinto', price: 45.0 },
  { id: 3, name: 'Refrigerante', price: 6.0 },
  { id: 4, name: 'Suco Natural', price: 8.0 }
];

const cart = [];

function renderMenu() {
  const container = document.querySelector('.container');
  menu.forEach(drink => {
    const div = document.createElement('div');
    div.className = 'drink';
    div.innerHTML = `
      <img src="https://source.unsplash.com/80x80/?drink" alt="${drink.name}">
      <div>
        <strong>${drink.name}</strong><br>
        R$ ${drink.price.toFixed(2)}
      </div>
      <button onclick="addToCart(${drink.id})">Adicionar</button>
    `;
    container.appendChild(div);
  });
}

function addToCart(id) {
  const drink = menu.find(d => d.id === id);
  cart.push(drink);
  renderCart();
}

function renderCart() {
  const list = document.querySelector('#cart-items');
  list.innerHTML = '';
  cart.forEach((item, index) => {
    const li = document.createElement('li');
    li.innerHTML = `${item.name} - R$ ${item.price.toFixed(2)} <button onclick="removeFromCart(${index})">X</button>`;
    list.appendChild(li);
  });
  const total = cart.reduce((sum, item) => sum + item.price, 0);
  document.querySelector('#total').textContent = total.toFixed(2);
}

function removeFromCart(index) {
  cart.splice(index, 1);
  renderCart();
}

function checkout() {
  if (cart.length === 0) {
    alert('Carrinho vazio!');
    return;
  }
  alert('Pedido realizado!');
  cart.length = 0;
  renderCart();
}

document.addEventListener('DOMContentLoaded', function() {
  renderMenu();
  renderCart();
});
