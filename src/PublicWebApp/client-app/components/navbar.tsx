import Link from 'next/link'

export default function Navbar() {
  return (
    <ul>
      <li><Link href="/">Home</Link></li>
      <li><Link href="/laptops">Laptops</Link></li>
    </ul>
  )
}
