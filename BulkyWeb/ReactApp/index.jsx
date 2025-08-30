import React from 'react';
import ReactDOM from 'react-dom/client';
import ProductDetails from './components/ProductDetails';

const container = document.getElementById('react-product-details');
const productId = container?.getAttribute('data-product-id');

const root = ReactDOM.createRoot(container);
root.render(<ProductDetails productId={productId} />);
