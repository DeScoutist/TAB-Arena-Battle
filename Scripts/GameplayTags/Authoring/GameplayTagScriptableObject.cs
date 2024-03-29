using UnityEngine;

namespace GameplayTags.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Tag")]
    public class GameplayTagScriptableObject : ScriptableObject
    {
        [SerializeField]
        private GameplayTagScriptableObject _parent;
        public GameplayTagScriptableObject Parent { get { return _parent; } }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            GameplayTagScriptableObject other = (GameplayTagScriptableObject) obj;
            // Сравниваем имена объектов
            return name == other.name;
        }

        public override int GetHashCode()
        {
            // Возвращаем хэш-код имени
            return name.GetHashCode();
        }
        
        /// <summary>
        /// <para>Check is this gameplay tag is a descendant of another gameplay tag.</para>
        /// By default, only 4 levels of ancestors are searched.
        /// </summary>
        /// <param name="other">Ancestor gameplay tag</param>
        /// <returns>True if this gameplay tag is a descendant of the other gameplay tag</returns>
        public bool IsDescendantOf(GameplayTagScriptableObject other, int nSearchLimit = 4)
        {
            int i = 0;
            GameplayTagScriptableObject tag = Parent;
            while (nSearchLimit > i++)
            {
                // tag will be invalid once we are at the root ancestor
                if (!tag) return false;

                // Match found, so we can return true
                if (tag == other) return true;

                // No match found, so try again with the next ancestor
                tag = tag.Parent;
            }

            // If we've exhausted the search limit, no ancestor was found
            return false;
        }

    }
}
