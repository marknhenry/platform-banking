import { useEffect, useMemo, useState } from 'react'
import './App.css'

type PageKey = 'home' | 'about' | 'contact'
type ThemeMode = 'light' | 'dark'

const pageContent: Record<PageKey, { heading: string; body: string }> = {
  home: {
    heading: 'Welcome to Platform Banking',
    body: 'A conversational, trustworthy banking experience that keeps control in your hands.',
  },
  about: {
    heading: 'About Us',
    body: 'We build transparent, policy-first digital banking tools for safer customer support and financial guidance.',
  },
  contact: {
    heading: 'Contact Us',
    body: 'Need help? Reach our support team at support@platform-banking.example.',
  },
}

function App() {
  const [activePage, setActivePage] = useState<PageKey>('home')
  const [theme, setTheme] = useState<ThemeMode>(() => {
    const storedTheme = window.localStorage.getItem('theme-mode')
    if (storedTheme === 'light' || storedTheme === 'dark') {
      return storedTheme
    }

    const supportsMatchMedia = typeof window.matchMedia === 'function'
    if (supportsMatchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
      return 'dark'
    }

    return 'light'
  })

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme)
    window.localStorage.setItem('theme-mode', theme)
  }, [theme])

  const currentContent = useMemo(() => pageContent[activePage], [activePage])

  const menuItems: Array<{ key: PageKey; label: string }> = [
    { key: 'home', label: 'Home' },
    { key: 'about', label: 'About' },
    { key: 'contact', label: 'Contact Us' },
  ]

  const toggleTheme = () => {
    setTheme((current) => (current === 'light' ? 'dark' : 'light'))
  }

  return (
    <div className="app-shell">
      <header className="site-header">
        <p className="brand">Platform Banking</p>
        <button type="button" className="theme-toggle" onClick={toggleTheme}>
          {theme === 'light' ? 'Switch to dark mode' : 'Switch to light mode'}
        </button>
      </header>

      <nav aria-label="Primary" className="main-nav">
        {menuItems.map((item) => (
          <button
            key={item.key}
            type="button"
            className={activePage === item.key ? 'nav-item active' : 'nav-item'}
            onClick={() => setActivePage(item.key)}
          >
            {item.label}
          </button>
        ))}
      </nav>

      <main className="content-panel" role="main">
        <h1>{currentContent.heading}</h1>
        <p>{currentContent.body}</p>
      </main>
    </div>
  )
}

export default App
