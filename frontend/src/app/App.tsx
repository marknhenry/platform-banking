import { useEffect, useMemo, useRef, useState } from 'react'
import type { FormEvent } from 'react'
import { bootstrapAuthSession } from '../services/apiClient'
import '../App.css'

type PageKey = 'home' | 'about' | 'contact'
type ThemeMode = 'light' | 'dark'
type ChatRole = 'assistant' | 'user'
type ChatMessage = {
  id: string
  role: ChatRole
  text: string
}

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

const parsePageFromHash = (): PageKey => {
  const hash = window.location.hash.replace('#', '').toLowerCase()
  if (hash === 'about' || hash === 'contact' || hash === 'home') {
    return hash
  }

  return 'home'
}

const createWelcomeMessage = (page: PageKey): ChatMessage => ({
  id: `welcome-${page}`,
  role: 'assistant',
  text: pageContent[page].body,
})

function App() {
  const [activePage, setActivePage] = useState<PageKey>(parsePageFromHash)
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false)
  const [messageDraft, setMessageDraft] = useState('')
  const [messages, setMessages] = useState<ChatMessage[]>(() => [createWelcomeMessage(activePage)])
  const [sessionUserId, setSessionUserId] = useState<string | null>(null)
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
    bootstrapAuthSession().then((session) => {
      if (session?.userId) {
        setSessionUserId(session.userId)
      }
    })
  }, [])

  useEffect(() => {
    const onHashChange = () => {
      setActivePage(parsePageFromHash())
      setIsMobileMenuOpen(false)
    }

    window.addEventListener('hashchange', onHashChange)
    return () => window.removeEventListener('hashchange', onHashChange)
  }, [])

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme)
    window.localStorage.setItem('theme-mode', theme)
  }, [theme])

  useEffect(() => {
    setMessages([createWelcomeMessage(activePage)])
    setMessageDraft('')
  }, [activePage])

  const chatLogEndRef = useRef<HTMLDivElement | null>(null)

  useEffect(() => {
    if (typeof chatLogEndRef.current?.scrollIntoView === 'function') {
      chatLogEndRef.current.scrollIntoView({ behavior: 'smooth' })
    }
  }, [messages])

  const currentContent = useMemo(() => pageContent[activePage], [activePage])

  const menuItems: Array<{ key: PageKey; label: string }> = [
    { key: 'home', label: 'Home' },
    { key: 'about', label: 'About' },
    { key: 'contact', label: 'Contact Us' },
  ]

  const toggleTheme = () => {
    setTheme((current) => (current === 'light' ? 'dark' : 'light'))
  }

  const handleMenuClick = (page: PageKey) => {
    window.location.hash = page
    setActivePage(page)
    setIsMobileMenuOpen(false)
  }

  const handleSendMessage = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const trimmedMessage = messageDraft.trim()
    if (!trimmedMessage) {
      return
    }

    const userMessage: ChatMessage = {
      id: `user-${Date.now()}`,
      role: 'user',
      text: trimmedMessage,
    }

    const assistantMessage: ChatMessage = {
      id: `assistant-${Date.now() + 1}`,
      role: 'assistant',
      text: `I can help with ${currentContent.heading.toLowerCase()}. You asked: "${trimmedMessage}".`,
    }

    setMessages((current) => [...current, userMessage, assistantMessage])
    setMessageDraft('')
  }

  return (
    <div className="app-shell">
      <header className="site-header">
        <button
          type="button"
          className="menu-toggle"
          aria-expanded={isMobileMenuOpen}
          aria-controls="primary-navigation"
          onClick={() => setIsMobileMenuOpen((current) => !current)}
        >
          <span className="menu-icon" aria-hidden="true">
            <span />
            <span />
            <span />
          </span>
          <span>{isMobileMenuOpen ? 'Close menu' : 'Menu'}</span>
        </button>

        <p className="brand">Platform Banking</p>

        <button
          type="button"
          className="theme-toggle"
          aria-label={theme === 'light' ? 'Switch to dark mode' : 'Switch to light mode'}
          onClick={toggleTheme}
        >
          {theme === 'light' ? 'Dark mode' : 'Light mode'}
        </button>
      </header>

      <nav
        id="primary-navigation"
        aria-label="Primary"
        className={isMobileMenuOpen ? 'main-nav is-open' : 'main-nav'}
      >
        {menuItems.map((item) => (
          <button
            key={item.key}
            type="button"
            className={activePage === item.key ? 'nav-item active' : 'nav-item'}
            onClick={() => handleMenuClick(item.key)}
          >
            {item.label}
          </button>
        ))}
      </nav>

      <main className="chat-panel" role="main">
        <header className="chat-header">
          <h1>{currentContent.heading}</h1>
          <p>
            Ask questions and get guidance in one place.
            {sessionUserId ? ` Signed in as ${sessionUserId}.` : ''}
          </p>
        </header>

        <section className="chat-log" aria-live="polite" aria-label="Conversation">
          {messages.map((message) => (
            <article
              key={message.id}
              className={message.role === 'user' ? 'message-row user' : 'message-row assistant'}
            >
              <p>{message.text}</p>
            </article>
          ))}
          <div ref={chatLogEndRef} />
        </section>

        <form className="chat-input-bar" onSubmit={handleSendMessage}>
          <label htmlFor="chat-message" className="sr-only">
            Message
          </label>
          <input
            id="chat-message"
            name="chat-message"
            type="text"
            value={messageDraft}
            onChange={(event) => setMessageDraft(event.target.value)}
            placeholder="Type your message..."
            autoComplete="off"
          />
          <button type="submit">Send</button>
        </form>
      </main>
    </div>
  )
}

export default App
