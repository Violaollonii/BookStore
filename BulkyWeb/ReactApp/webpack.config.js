const path = require('path');

module.exports = {
    entry: './index.jsx',
    output: {
        path: path.resolve(__dirname, '../wwwroot/js'),
        filename: 'productDetails.bundle.js',
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                },
            },
        ],
    },
    resolve: {
        extensions: ['.js', '.jsx'],
    },
    mode: 'development',
};
