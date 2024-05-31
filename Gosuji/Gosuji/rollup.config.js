import resolve from 'rollup-plugin-node-resolve';
import commonjs from 'rollup-plugin-commonjs';
import multiEntry from '@rollup/plugin-multi-entry';
import terser from '@rollup/plugin-terser';

export default {
    input: 'wwwroot/js/pages/josekis/**/*.js',
    output: {
        file: 'wwwroot/js/pages/josekis/bundle.min.js',
        format: 'esm',
        sourcemap: true,
        plugins: [terser()]
    },
    plugins: [
        multiEntry(),
        resolve(), // Resolve modules from node_modules
        commonjs() // Convert CommonJS modules to ES6
    ]
};