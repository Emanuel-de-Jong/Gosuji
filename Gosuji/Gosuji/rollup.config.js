import terser from '@rollup/plugin-terser';

const createPageBundle = (filePath, fileName = filePath) => ({
    input: 'Resources/js/pages/' + filePath + '/' + fileName + '.js',
    output: {
        file: 'wwwroot/js/pages/' + filePath + '/bundle.js',
        format: 'es',
        sourcemap: false
    },
    plugins: [
        // terser({
        //     compress: {
        //         unused: false,       // Prevents removal of unused variables and functions
        //         side_effects: false  // Avoids dropping code that Terser thinks has no side effects
        //     },
        // })
    ]
});

let config = [
    {
        input: 'Resources/js/custom.js',
        output: {
            file: 'wwwroot/js/bundle.js',
            format: 'iife',
            sourcemap: false
        },
        plugins: [
            terser({
                compress: {
                    unused: false,       // Prevents removal of unused variables and functions
                    side_effects: false  // Avoids dropping code that Terser thinks has no side effects
                },
            })
        ]
    },
    createPageBundle('trainer', 'init'),
    createPageBundle('josekis'),
    createPageBundle('profile'),
    createPageBundle('cms'),
];

export default config;
