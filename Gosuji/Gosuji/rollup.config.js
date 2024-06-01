import terser from '@rollup/plugin-terser';

export default {
    input: 'wwwroot/js/pages/josekis/josekis.js',
    output: {
        file: 'wwwroot/js/pages/josekis/bundle.min.js',
        format: 'es',
        sourcemap: true
    },
    plugins: [
        terser()
    ]
};