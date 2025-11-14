# Copyright Analysis - Tyler Gehring

## Example of Code Reuse that Violates Copyright Law

### Utilized Asset
**TNT Bomb Sprite** (`Assets/src/TylerGehring/sprites/bomb.png`)

### Source of Asset
The bomb sprite was taken from a paid asset pack on the Unity Asset Store called 'Bombs and Explosive Barrels' by the artist "Sandman." It was used without purchasing 
the commercial license which is needed to legally abide to copyright.

### Copyright Violation Explanation

#### How It Violates Copyright
The original artist/company holds copyright protection for this sprite because:
1. **Original Creative Expression**: The sprite demonstrates artistic choices in design, color palette, shading, and style that constitute creative expression protected under copyright law.
2. **Fixed in Tangible Medium**: The artwork is fixed in a digital image file format (.png), meeting the requirement for copyright protection.
3. **No License Granted**: The sprite was used without obtaining permission, a license, or purchasing usage rights from the copyright holder.
4. **Unauthorized Reproduction**: By copying and integrating the sprite into my game project, I reproduced and distributed the copyrighted work without authorization.

### Code Integration

The copyrighted sprite is integrated into my codebase through the following mechanisms:

1. **File Location**: `Assets/src/TylerGehring/sprites/bomb.png`

2. **Unity Integration**: The sprite is imported into Unity as a Sprite asset and assigned to game objects through the Inspector or programmatically.

3. **Code Reference**: The `TNT.cs` script uses Unity's SpriteRenderer component to display this copyrighted image:
   ```csharp
   private SpriteRenderer _spriteRenderer;
   
   protected override void OnEnable()
   {
       base.OnEnable();
       _CacheComponents();  // This caches the SpriteRenderer
       // The SpriteRenderer displays the copyrighted bomb.png sprite
   }
   ```

4. **Runtime Display**: When a player uses TNT in the game, the copyrighted bomb sprite is rendered on-screen, constituting public display of the copyrighted work.

### Legal Implications if Marketed

If I were to market this game with the copyrighted sprite included, potential legal consequences include:

1. **Copyright Infringement Lawsuit**: The copyright holder could sue for statutory damages ranging from $750 to $30,000 per work infringed, or up to $150,000 if willful infringement is proven.

2. **Cease and Desist**: I would likely receive a cease and desist letter requiring immediate removal of the sprite and potentially removal of the game from sale.

3. **Injunction**: A court could issue an injunction preventing further distribution of the game.

4. **Actual Damages**: The copyright holder could claim actual damages including their lost revenue and any profits I earned from the game.

5. **Criminal Penalties**: In extreme cases involving willful infringement for commercial gain, criminal copyright infringement charges could be filed.

## Fair Use Analysis

Despite the copyright violation, I could argue Fair Use under 17 U.S.C. § 107 based on the following four factors:

### 1. Purpose and Character of Use (Transformative Nature)

**Argument FOR Fair Use:**
- **Transformative Use**: While I cannot claim educational/nonprofit use if marketing the game commercially, my use is arguably transformative. The sprite is not used as standalone artwork but is integrated into an interactive game experience with new context, meaning, and purpose.
- **New Expression**: The sprite becomes part of a larger narrative experience within SubTerra, where it represents an interactive tool with gameplay mechanics (explosive physics, fuse timing, radius calculations) that add new expression to the original static image.
- **Different Medium**: The sprite transitions from a static image to an interactive game element with collision detection, physics simulation, and player interaction.

**Argument AGAINST Fair Use:**
- **Commercial Nature**: Marketing the game is explicitly commercial use, which weighs heavily against fair use.
- **Limited Transformation**: The sprite's visual appearance remains unchanged; I'm primarily using it for its original decorative/representational purpose (to depict a bomb).

### 2. Nature of the Copyrighted Work

**Argument FOR Fair Use:**
- **Factual Elements**: A bomb sprite depicts a real-world object (TNT/explosives) and contains factual elements about what bombs look like, limiting the scope of copyright protection.
- **Common Visual Tropes**: Bomb imagery in games follows established conventions (round with fuse, red/black colors), suggesting less creative originality.

**Argument AGAINST Fair Use:**
- **Creative Expression**: The specific artistic style, color choices, shading techniques, and visual design are creative expressions deserving full copyright protection.
- **Published Work**: The work was presumably published and distributed, providing it with full copyright protection.

### 3. Amount and Substantiality of the Portion Taken

**Argument FOR Fair Use:**
- **Small Component**: The bomb sprite is one small visual element among many assets in the SubTerra game. It is not a central or featured component of the game's marketing or core experience.
- **De Minimis Use**: The sprite occupies minimal screen space during gameplay and appears only when players use TNT items, which may be infrequent.

**Argument AGAINST Fair Use:**
- **Entire Work Used**: I used the entire copyrighted sprite image, not just a portion. Courts generally disfavor using the entirety of a copyrighted work.
- **Qualitative Importance**: Even though small in file size, the sprite serves an important functional role in representing TNT items in the game.

### 4. Effect on the Potential Market

**Argument FOR Fair Use:**
- **Different Markets**: SubTerra (a cave exploration/mining game) operates in a different market than the original sprite's intended market (game asset pack sales or original game sales).
- **No Market Substitution**: Players purchasing SubTerra are not buying it as a substitute for purchasing the original sprite or the source material. The markets do not overlap significantly.
- **No Harm to Licensing Market**: The use of this single sprite is unlikely to impact the copyright holder's ability to license their work to others, as SubTerra is not competing in the asset licensing market.
- **Minimal Market Impact**: Given that this is one sprite among potentially dozens or hundreds in the game, its presence is unlikely to affect the original creator's market significantly.

**Argument AGAINST Fair Use:**
- **Lost Licensing Revenue**: The copyright holder could have licensed the sprite to me for use in SubTerra. My unauthorized use deprives them of potential licensing fees.
- **Precedent Concerns**: If my fair use claim succeeds, it could encourage others to use the sprite without permission, harming the licensing market.

## Conclusion

### Fair Use Verdict (Realistic Assessment)
Based on the four-factor analysis, **my fair use claim would likely fail in court** for the following reasons:

1. The commercial nature of marketing the game weighs heavily against fair use (Factor 1).
2. I used the entire copyrighted work rather than a portion (Factor 3).
3. While markets differ, I deprived the copyright holder of potential licensing revenue (Factor 4).
4. The transformative nature argument is weak because the sprite's appearance and primary function (visual representation) remain unchanged (Factor 1).

**Strongest Arguments**: Factors 2 and 4 (different markets, minimal market impact) provide the best defense.

**Weakest Arguments**: Factor 1 (commercial use, limited transformation) and Factor 3 (entire work used) would likely doom the fair use claim.

### Recommended Action
To legally market SubTerra with this sprite, I should:
1. **Create original artwork** or commission an artist to create a unique bomb sprite
2. **Purchase a license** from the copyright holder for commercial use
3. **Use royalty-free assets** from sources that explicitly grant commercial usage rights (e.g., Creative Commons CC0, public domain, purchased asset packs with commercial licenses)
4. **Seek permission** from the copyright holder with a written agreement for use

### Educational Note
This analysis demonstrates how copyright law protects creative works and how fair use is a nuanced, context-dependent defense that rarely succeeds for commercial uses of entire copyrighted works. Understanding these principles is crucial for game developers to avoid legal liability.
