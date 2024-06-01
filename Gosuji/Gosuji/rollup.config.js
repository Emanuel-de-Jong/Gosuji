import terser from '@rollup/plugin-terser';

const createPageBundle = (input, output) => ({
    input,
    output: {
        ...output,
        format: 'es',
        sourcemap: true
    },
    plugins: [
        terser()
    ]
});

export default [
    createPageBundle('wwwroot/js/pages/josekis/josekis.js', {
        file: 'wwwroot/js/pages/josekis/bundle.min.js'
    }),
    createPageBundle('wwwroot/js/pages/cms/cms.js', {
        file: 'wwwroot/js/pages/cms/bundle.min.js'
    }),
];