import React from 'react';

const CartDropdown = ({ cart, total, numItems, clearCart }) => {
    return (
        <div className="nav-item dropdown">
            <a className="nav-link dropdown-toggle cart-link" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                🛒
                <span className="badge bg-danger" id="cart-count">{numItems}</span>
            </a>
            <ul className="dropdown-menu">
                {cart.length === 0 ? (
                    <li><span className="dropdown-item">Your cart is empty</span></li>
                ) : (
                    cart.map((item, index) => (
                        <li key={index} className="dropdown-item">
                            {item.name} - ${item.price} x {item.quantity}
                        </li>
                    ))
                )}
                {cart.length > 0 && (
                    <>
                        <li><hr className="dropdown-divider" /></li>
                        <li><span className="dropdown-item">Total: ${total.toFixed(2)}</span></li>
                        <li>
                            <button className="dropdown-item btn btn-danger" onClick={clearCart}>
                                🗑️ Clear Cart
                            </button>
                        </li>
                    </>
                )}
            </ul>
        </div>
    );
};

export default CartDropdown;
