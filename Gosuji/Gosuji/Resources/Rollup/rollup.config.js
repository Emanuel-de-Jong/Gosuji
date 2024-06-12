import terser from '@rollup/plugin-terser';

const createPageBundle = (filePath, fileName = filePath) => ({
    input: '../js/pages/' + filePath + '/' + fileName + '.js',
    output: {
        file: '../../wwwroot/js/pages/' + filePath + '/bundle.js',
        format: 'es',
        sourcemap: true
    },
    plugins: [
        terser({
            compress: {
                unused: false,       // Prevents removal of unused variables and functions
                side_effects: false  // Avoids dropping code that Terser thinks has no side effects
            },
        })
    ]
});

let config = [
    // Besogo minify
    {
        input: '../js/libs/besogo.all.js',
        output: {
            file: '../../wwwroot/js/libs/besogo.all.js',
            format: 'iife',
            sourcemap: true
        },
        plugins: [
            terser({
                compress: {
                    unused: false,
                    side_effects: false
                },
            })
        ]
    },

    // Custom globals bundle
    {
        input: '../js/custom.js',
        output: {
            file: '../../wwwroot/js/bundle.js',
            format: 'iife',
            sourcemap: true
        },
        plugins: [
            terser({
                compress: {
                    unused: false,
                    side_effects: false
                },
            })
        ]
    },

    // Page bundles
    createPageBundle('trainer'),
    createPageBundle('josekis'),
    createPageBundle('profile'),
    createPageBundle('cms'),
];

export default config;
