import React, { useEffect, useState } from 'react';

function ProductDetails({ productId }) {
    const [product, setProduct] = useState(null);

    useEffect(() => {
        fetch(`/api/product/${productId}`)
            .then(res => {
                if (!res.ok) throw new Error("Nuk u gjet produkti");
                return res.json();
            })
            .then(data => setProduct(data))
            .catch(err => {
                console.error("Gabim gjatë ngarkimit:", err);
                setProduct(null);
            });
    }, [productId]);

    if (!product) return <p>Loading...</p>;

    return (
        <div className="container px-4 px-lg-5 my-5">
            <div className="row gx-4 gx-lg-5 align-items-start">
                <div className="col-md-6">
                    <img
                        src={product.productImages?.[0]?.imageUrl || "https://placehold.co/600x700"}
                        alt={product.title}
                        className="card-img-top mb-5 mb-md-0"
                    />
                </div>
                <div className="col-md-6">
                    <div className="small mb-1">ISBN: {product.isbn}</div>
                    <h1 className="display-5 fw-bolder">{product.title}</h1>
                    <div className="text-muted mb-2">by <b>{product.author}</b></div>
                    <span className="badge bg-secondary mb-3">{product.category?.name}</span>
                    <div className="fs-5 mb-5">
                        <span className="text-decoration-line-through">€{product.listPrice}</span>
                        <br />
                        <span className="text-dark">From: €{product.price100}</span>
                    </div>
                    <p className="lead" dangerouslySetInnerHTML={{ __html: product.description }}></p>
                </div>
            </div>
        </div>
    );
}

export default ProductDetails;
