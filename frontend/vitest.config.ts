import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./tests/unit/setupTests.ts'],
    include: ['tests/unit/**/*.test.ts?(x)'],
  },
})
