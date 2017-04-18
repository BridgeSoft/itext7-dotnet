using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// <p>
    /// This class is used to manage waiting tags status.
    /// </summary>
    /// <remarks>
    /// <p>
    /// This class is used to manage waiting tags status.
    /// Any tag in the structure tree could be marked as "waiting". This status indicates that
    /// tag is not yet finished and therefore should not be flushed or removed if page tags are
    /// flushed or removed or if parent tags are flushed.
    /// </p>
    /// <p>
    /// Waiting status of tags is defined by the association with arbitrary objects instances.
    /// This mapping is one to one: for every waiting tag there is always exactly one associated object.
    /// </p>
    /// Waiting status could also be perceived as a temporal association of the object to some particular tag.
    /// </remarks>
    public class WaitingTagsManager {
        private IDictionary<Object, PdfStructElem> associatedObjToWaitingTag;

        private IDictionary<PdfDictionary, Object> waitingTagToAssociatedObj;

        internal WaitingTagsManager() {
            associatedObjToWaitingTag = new Dictionary<Object, PdfStructElem>();
            waitingTagToAssociatedObj = new Dictionary<PdfDictionary, Object>();
        }

        /// <summary>
        /// Assigns waiting status to the tag at which given
        /// <see cref="TagTreePointer"/>
        /// points, associating it with the given
        /// <see cref="System.Object"/>
        /// . If current tag of the given
        /// <see cref="TagTreePointer"/>
        /// is already waiting, then after this method call
        /// it's associated object will change to the one passed as the argument and the old one will not longer be
        /// an associated object.
        /// </summary>
        /// <param name="pointer">
        /// a
        /// <see cref="TagTreePointer"/>
        /// pointing at a tag which is desired to be marked as waiting.
        /// </param>
        /// <param name="associatedObj">an object that is to be associated with the waiting tag. A null value is forbidden.
        ///     </param>
        /// <returns>
        /// the previous associated object with the tag if it have already had waiting status,
        /// or null if it was not waiting tag.
        /// </returns>
        public virtual Object AssignWaitingTagStatus(TagTreePointer pointer, Object associatedObj) {
            if (associatedObj == null) {
                throw new ArgumentNullException();
            }
            return SaveAssociatedObjectForWaitingTag(associatedObj, pointer.GetCurrentStructElem());
        }

        /// <summary>
        /// Checks if there is waiting tag which status was assigned using given
        /// <see cref="System.Object"/>
        /// .
        /// </summary>
        /// <param name="obj">
        /// an
        /// <see cref="System.Object"/>
        /// which is to be checked if it is associated with any waiting tag. A null value is forbidden.
        /// </param>
        /// <returns>true if object is currently associated with some waiting tag.</returns>
        public virtual bool IsObjectAssociatedWithWaitingTag(Object obj) {
            if (obj == null) {
                throw new ArgumentNullException();
            }
            return associatedObjToWaitingTag.ContainsKey(obj);
        }

        /// <summary>
        /// Moves given
        /// <see cref="TagTreePointer"/>
        /// to the waiting tag which is associated with the given object.
        /// If the passed object is not associated with any waiting tag,
        /// <see cref="TagTreePointer"/>
        /// position won't change.
        /// </summary>
        /// <param name="tagPointer">
        /// a
        /// <see cref="TagTreePointer"/>
        /// which position in the tree is to be changed to the
        /// waiting tag in case of the successful call.
        /// </param>
        /// <param name="associatedObject">
        /// an object which is associated with the waiting tag to which
        /// <see cref="TagTreePointer"/>
        /// is to be moved.
        /// </param>
        /// <returns>
        /// true if given object is actually associated with the waiting tag and
        /// <see cref="TagTreePointer"/>
        /// was moved
        /// in order to point at it.
        /// </returns>
        public virtual bool MovePointerToWaitingTag(TagTreePointer tagPointer, Object associatedObject) {
            if (associatedObject == null) {
                return false;
            }
            PdfStructElem waitingStructElem = associatedObjToWaitingTag.Get(associatedObject);
            if (waitingStructElem != null) {
                tagPointer.SetCurrentStructElem(waitingStructElem);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets an object that is associated with the tag (if there is one) at which given
        /// <see cref="TagTreePointer"/>
        /// points.
        /// Essentially, this method could be used as indication that current tag has waiting status.
        /// </summary>
        /// <param name="pointer">
        /// a
        /// <see cref="TagTreePointer"/>
        /// which points at the tag for which associated object is to be retrieved.
        /// </param>
        /// <returns>
        /// an object that is associated with the tag at which given
        /// <see cref="TagTreePointer"/>
        /// points, or null if
        /// current tag of the
        /// <see cref="TagTreePointer"/>
        /// is not a waiting tag.
        /// </returns>
        public virtual Object GetAssociatedObject(TagTreePointer pointer) {
            return GetObjForStructDict(pointer.GetCurrentStructElem().GetPdfObject());
        }

        /// <summary>Removes waiting status of the tag which is associated with the given object.</summary>
        /// <remarks>
        /// Removes waiting status of the tag which is associated with the given object.
        /// <p>NOTE: if parent of the waiting tag is already flushed, the tag and it's children
        /// (unless they are waiting tags on their own) will be also immediately flushed right after
        /// the waiting status removal.</p>
        /// </remarks>
        /// <param name="associatedObject">an object which association with the waiting tag is to be removed.</param>
        /// <returns>true if object was actually associated with some tag and it's association was removed.</returns>
        public virtual bool RemoveWaitingTagStatus(Object associatedObject) {
            if (associatedObject != null) {
                PdfStructElem structElem = associatedObjToWaitingTag.JRemove(associatedObject);
                RemoveWaitingStatusAndFlushIfParentFlushed(structElem);
                return structElem != null;
            }
            return false;
        }

        /// <summary>Removes waiting status of all waiting tags by removing association with objects.</summary>
        /// <remarks>
        /// Removes waiting status of all waiting tags by removing association with objects.
        /// <p>NOTE: if parent of the waiting tag is already flushed, the tag and it's children
        /// will be also immediately flushed right after the waiting status removal.</p>
        /// </remarks>
        public virtual void RemoveWaitingStatusOfAllTags() {
            foreach (PdfStructElem structElem in associatedObjToWaitingTag.Values) {
                RemoveWaitingStatusAndFlushIfParentFlushed(structElem);
            }
            associatedObjToWaitingTag.Clear();
        }

        internal virtual PdfStructElem GetStructForObj(Object associatedObj) {
            return associatedObjToWaitingTag.Get(associatedObj);
        }

        internal virtual Object GetObjForStructDict(PdfDictionary structDict) {
            return waitingTagToAssociatedObj.Get(structDict);
        }

        internal virtual Object SaveAssociatedObjectForWaitingTag(Object associatedObj, PdfStructElem structElem) {
            associatedObjToWaitingTag.Put(associatedObj, structElem);
            return waitingTagToAssociatedObj.Put(structElem.GetPdfObject(), associatedObj);
        }

        /// <returns>parent of the flushed tag</returns>
        internal virtual IPdfStructElem FlushTag(PdfStructElem tagStruct) {
            Object associatedObj = waitingTagToAssociatedObj.JRemove(tagStruct.GetPdfObject());
            if (associatedObj != null) {
                associatedObjToWaitingTag.JRemove(associatedObj);
            }
            IPdfStructElem parent = tagStruct.GetParent();
            FlushStructElementAndItKids(tagStruct);
            return parent;
        }

        private void FlushStructElementAndItKids(PdfStructElem elem) {
            if (waitingTagToAssociatedObj.ContainsKey(elem.GetPdfObject())) {
                return;
            }
            foreach (IPdfStructElem kid in elem.GetKids()) {
                if (kid is PdfStructElem) {
                    FlushStructElementAndItKids((PdfStructElem)kid);
                }
            }
            elem.Flush();
        }

        private void RemoveWaitingStatusAndFlushIfParentFlushed(PdfStructElem structElem) {
            if (structElem != null) {
                waitingTagToAssociatedObj.JRemove(structElem.GetPdfObject());
                IPdfStructElem parent = structElem.GetParent();
                if (parent is PdfStructElem && ((PdfStructElem)parent).IsFlushed()) {
                    FlushStructElementAndItKids(structElem);
                }
            }
        }
    }
}
