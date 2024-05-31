import resolve from 'rollup-plugin-node-resolve';
import commonjs from 'rollup-plugin-commonjs';
import terser from '@rollup/plugin-terser';

export default {
    input: 'wwwroot/js/pages/josekis/josekis.js',
    output: {
        file: 'wwwroot/js/pages/josekis/bundle.min.js',
        format: 'esm',
        sourcemap: true,
        plugins: [terser()]
    },
    plugins: [
        resolve(), // Resolve modules from node_modules
        commonjs() // Convert CommonJS modules to ES6
    ]
};