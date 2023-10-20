var path = require('path');
module.exports = {
    entry: './index.js',
    output: {
        filename: 'wwwroot/js/bundle.js',
        path: path.resolve(__dirname)
    },
    mode: 'production'
};